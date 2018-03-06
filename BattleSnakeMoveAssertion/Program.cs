using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion
{
    internal class Program
    {
        private static int _errorCount;
        private const int MaxAllowedErrors = 3;
        private static readonly string ValidDirectionsMessage = "Valid directions: " + string.Join("|", Api.Move.All.Select(direction => direction));
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromMilliseconds(1000) };

        private static void Main(string[] args)
        {
            // Validate the command line arguments. Exit if they are invalid.
            Uri snakeMoveUri = null; // Command line argument 1. Example: http://snakeai.com/api
            string moveAssertionDirectory = null; // Command line argument 2. Example: C:\Users\User\DirectoryName
            if (!GetCommandLineArguments(args, ref snakeMoveUri, ref moveAssertionDirectory))
            {
                return;
            }

            // Process each move assertion file in the move assertion directory until completion or an exception occurs.
            int processedFileCount = 0;
            int passCount = 0;
            int failCount = 0;
            foreach (string moveAssertionFileFullPath in Directory.EnumerateFiles(moveAssertionDirectory, "*.move"))
            {
                string moveAssertionFile = Path.GetFileNameWithoutExtension(moveAssertionFileFullPath);

                // Get and validate the three segments of this move assertion filename. Exit if any are invalid.
                string[] desiredDirections = null; // First segment. Example: up-down
                string badDirection = null; // Second segment. Example: right
                string snakeDownGameId = null; // Third segment. Example: 213-sad-23-12-das-123
                if (!GetMoveAssertionFilenameSegments(moveAssertionFile, ref desiredDirections, ref badDirection, ref snakeDownGameId))
                {
                    return;
                }

                // Get and validate the world JSON from the file. Exit if it is invalid.
                string worldJson = File.ReadAllText(moveAssertionFileFullPath);
                if (!(JsonConvert.DeserializeObject(worldJson, typeof(Messages.World)) is Messages.World))
                {
                    Console.WriteLine(moveAssertionFile + ": contains invalid World JSON. Please review the API.");
                }

                // Get the snake's move for this world:
                Messages.MoveResponse moveResponse;
                try
                {
                    // Create the move request content.
                    StringContent moveRequestContent = new StringContent(worldJson, Encoding.UTF8, "application/json");

                    // Send the move request and await the response.
                    Task<HttpResponseMessage> moveRequestTask = Task.Run(() => HttpClient.PostAsync(snakeMoveUri, moveRequestContent));
                    moveRequestTask.Wait();
                    HttpResponseMessage moveResponseMessage = moveRequestTask.Result;

                    // If an unsuccessful status code was received, continue (unless the maximum amount of move request failures is reached).
                    if (!moveResponseMessage.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Your snake returned an unsuccessful status code for the move assertion file: " + moveAssertionFile);
                        if (ReachedMaxAllowedMoveRequestFailures())
                        {
                            return;
                        }
                        continue;
                    }

                    // Read and assign the snake's move response.
                    Task<string> moveResponseTask = Task.Run(() => moveResponseMessage.Content.ReadAsStringAsync());
                    moveResponseTask.Wait();
                    string moveResponseString = moveResponseTask.Result;
                    moveResponse = (Messages.MoveResponse)JsonConvert.DeserializeObject(moveResponseString, typeof(Messages.MoveResponse));
                }

                // If an exception occured, continue (unless the maximum amount of move request failures is reached).
                catch (Exception ex)
                {
                    Console.WriteLine("An exception occured while getting your snake's move response for the move assertion file: " +
                                      moveAssertionFile + "\n" + ex);
                    if (ReachedMaxAllowedMoveRequestFailures())
                    {
                        return;
                    }
                    continue;
                }

                // Validate the snake's move direction. If it is invalid, continue (unless the maximum amount of move request failures is reached).
                if (!Api.Move.All.Contains(moveResponse.Move))
                {
                    Console.WriteLine("Your snake responded with an invalid direction for the move assertion file: " + moveAssertionFile);
                    if (ReachedMaxAllowedMoveRequestFailures())
                    {
                        return;
                    }
                    continue;
                }

                // Assert the move.
                if (moveResponse.Move == badDirection)
                {
                    Console.WriteLine("FAIL: " + moveResponse.Move + "\t(" + moveAssertionFile + ")");
                    failCount++;
                }
                else if (desiredDirections.Contains(moveResponse.Move))
                {
                    Console.WriteLine("PASS: " + moveResponse.Move + "\t(" + moveAssertionFile + ")");
                    passCount++;
                }
                else
                {
                    Console.WriteLine("FAIL: " + moveResponse.Move + "\t(" + moveAssertionFile + ")");
                    failCount++;
                }

                processedFileCount++;
            }

            Console.WriteLine("\nDone processing " + processedFileCount + " move assertion files.\nPASS: " + passCount + "\nFAIL: " + failCount +
                              "\nMove request failures: " + _errorCount + " (maximum allowed is " + MaxAllowedErrors + ")");
        }

        private static void WriteMessageWithUsageToConsole(string message)
        {
            Console.WriteLine(message + "\nUsage: <snake absolute base URI> <absolute path to directory with move assertion files>\nExample: " +
                              "http://snakeai.com/api C:\\Users\\Bob\\Documents\\BattleSnake_Moves");
        }

        private static void WriteMessageWithFilenameFormatToConsole(string message)
        {
            Console.WriteLine(message + "\nFilename format: <desired directions separated by ->_<bad direction>_<SnakeDown game ID>.move\n" +
                              "Examples: up_down_123.move, up-down_right_123.move");
        }

        private static bool GetCommandLineArguments(IReadOnlyList<string> args, ref Uri snakeMoveUri, ref string moveAssertionDirectory)
        {
            // Ensure that two command line arguments were provided.
            if (args is null || args.Count != 2)
            {
                WriteMessageWithUsageToConsole("Must provide the two required command line arguments.");
                return false;
            }

            // Ensure that the first command line argument (the snake absolute base URI) is well-formed.
            if (!Uri.IsWellFormedUriString(args[0] + "/move", UriKind.Absolute))
            {
                WriteMessageWithUsageToConsole("Provided snake absolute base URI is not a well-formed absolute URI string.");
                return false;
            }
            snakeMoveUri = new Uri(args[0] + "/move");
            if (!(snakeMoveUri.Scheme == Uri.UriSchemeHttp || snakeMoveUri.Scheme == Uri.UriSchemeHttps))
            {
                WriteMessageWithUsageToConsole("Provided snake absolute base URI scheme must be HTTP or HTTPS.");
                return false;
            }

            // Ensure that the second command line argument (the absolute path to the directory with the move assertion files) is well-formed.
            try
            {
                moveAssertionDirectory = Path.GetFullPath(args[1]);
            }
            catch
            {
                WriteMessageWithUsageToConsole("Provided absolute path to the directory with the move assertion files is not well-formed.");
                return false;
            }

            return true;
        }

        private static bool GetMoveAssertionFilenameSegments(string moveAssertionFilename, ref string[] desiredDirections, ref string badDirection, ref string snakeDownGameId)
        {
            // Get and validate the three segments of the current move assertion filename.
            string[] moveAssertionFilenameSegments = moveAssertionFilename.Split('_');
            if (moveAssertionFilenameSegments is null || moveAssertionFilenameSegments.Length != 3)
            {
                WriteMessageWithFilenameFormatToConsole(moveAssertionFilename + " is not a well-formed move assertion filename.");
                return false;
            }

            // Validate that the first segment (the desired direction(s) to move) is valid.
            desiredDirections = moveAssertionFilenameSegments[0].Split('-');
            if (desiredDirections is null || desiredDirections.Any(desiredDirection => !Api.Move.All.Contains(desiredDirection)))
            {
                WriteMessageWithFilenameFormatToConsole(moveAssertionFilename + ": " + moveAssertionFilenameSegments[0] +
                                                        " contains an invalid direction. " + ValidDirectionsMessage);
                return false;
            }

            // Validate that the second segment (the "bad" direction that your snake moved) is valid.
            badDirection = moveAssertionFilenameSegments[1];
            if (!Api.Move.All.Contains(badDirection))
            {
                WriteMessageWithFilenameFormatToConsole(moveAssertionFilename + ": " + badDirection +
                                                        " is not a valid direction. " + ValidDirectionsMessage);
                return false;
            }

            // Not validating the third argument (it's just supposed to be the SnakeDown game ID).
            snakeDownGameId = moveAssertionFilenameSegments[2];

            return true;
        }

        private static bool ReachedMaxAllowedMoveRequestFailures()
        {
            if (++_errorCount == MaxAllowedErrors)
            {
                Console.WriteLine("Your snake has failed to respond to " + _errorCount + " move requests. Ending the program.");
                return true;
            }
            return false;
        }
    }
}
