using BattleSnakeMoveAssertion.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using static BattleSnakeMoveAssertion.Models.Messages;

namespace BattleSnakeMoveAssertion
{
    internal class Program
    {
        #region Main
        private static void Main(string[] args)
        {
            // Used for exiting the program early if too many errors occur (e.g., improperly formatted move assertion filename or invalid file content).
            const int MaxAllowedErrors = 3;
            int errorCount = 0;
            bool ReachedMaxAllowedMoveRequestFailures()
            {
                if (++errorCount == MaxAllowedErrors)
                {
                    Console.WriteLine("Your snake has failed to respond to " + errorCount + " move requests. Ending the program.");
                    return true;
                }
                return false;
            }

            // Validate the command line arguments. Exit if they are invalid.
            Uri snakeMoveUri = null; // Command line argument 1. Example: http://snakeai.com/api
            string moveAssertionDirectory = null; // Command line argument 2. Example: C:\Users\User\DirectoryName
            if (!Helpers.GetCommandLineArguments(args, ref snakeMoveUri, ref moveAssertionDirectory))
            {
                return;
            }

            // Process each move assertion file in the move assertion directory until completion.
            int processedFilesCount = 0;
            int passCount = 0;
            int failCount = 0;
            foreach (string moveAssertionFileFullPath in Directory.EnumerateFiles(moveAssertionDirectory, "*.json"))
            {
                string moveAssertionFile = Path.GetFileNameWithoutExtension(moveAssertionFileFullPath);

                // Get and validate the three segments of this move assertion filename. Exit if any are invalid.
                string[] desiredDirections = null; // First segment. Example: up-down
                string badDirection = null; // Second segment. Example: right
                string snakeDownGameId = null; // Third segment. Example: 213-sad-23-12-das-123
                if (!Helpers.GetMoveAssertionFilenameSegments(moveAssertionFile, ref desiredDirections, ref badDirection, ref snakeDownGameId))
                {
                    return;
                }

                // Get and validate the world JSON from the file. Exit if it is invalid.
                string worldJson = File.ReadAllText(moveAssertionFileFullPath);
                try
                {
                    if (!(JsonConvert.DeserializeObject(worldJson, typeof(MoveRequest)) is MoveRequest))
                    {
                        throw null;
                    }
                }
                catch
                {
                    Console.WriteLine(moveAssertionFile + ": contains invalid World JSON. Please review the API.");
                    return;
                }

                // Get the snake's move for this world:
                MoveResponse moveResponse;
                try
                {
                    // Create the move request content.
                    StringContent moveRequestContent = new StringContent(worldJson, Encoding.UTF8, "application/json");

                    // Send the move request and await the response.
                    HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromMilliseconds(1000) };
                    Task<HttpResponseMessage> moveRequestTask = Task.Run(() => httpClient.PostAsync(snakeMoveUri, moveRequestContent));
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
                    moveResponse = (MoveResponse)JsonConvert.DeserializeObject(moveResponseString, typeof(MoveResponse));
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

                processedFilesCount++;
            }

            Console.WriteLine("\nDone processing " + processedFilesCount + " move assertion files.\nPASS: " + passCount + "\nFAIL: " + failCount +
                              "\nMove request failures: " + errorCount + " (maximum allowed is " + MaxAllowedErrors + ")");
        }
        #endregion
    }
}
