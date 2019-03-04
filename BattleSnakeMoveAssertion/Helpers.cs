using BattleSnakeMoveAssertion.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BattleSnakeMoveAssertion
{
    internal static class Helpers
    {
        #region Fields
        private const string UsageMessage = "\nUsage: <snake absolute base URI> <absolute path to directory with move assertion files>\nExample: " +
                              "http://battlesnakeai.com/api C:\\Users\\Bob\\Documents\\BattleSnake_Moves";
        private const string FilenameFormatMessage = "\nFilename format: <desired directions separated by ->_<bad direction>_<SnakeDown game ID>.move\n" +
                              "Examples: up_down_123.move, up-left_right_123.move";
        private static readonly string ValidDirectionsMessage = "Valid directions: " + string.Join("|", Api.Move.All.Select(direction => direction));
        #endregion

        #region Functions
        internal static void WriteMessageWithUsageToConsole(string message)
        {
            Console.WriteLine(message + UsageMessage);
        }

        internal static void WriteMessageWithFilenameFormatToConsole(string message)
        {
            Console.WriteLine(message + FilenameFormatMessage);
        }

        internal static bool GetCommandLineArguments(IReadOnlyList<string> args, ref Uri snakeMoveUri, ref string moveAssertionDirectory)
        {
            // Ensure that two command line arguments were provided.
            if (args is null || args.Count != 2)
            {
                WriteMessageWithUsageToConsole("You must provide the two required command line arguments.");
                return false;
            }

            // Ensure that the first command line argument (the snake absolute base URI) is well-formed.
            if (!Uri.IsWellFormedUriString(args[0] + "/move", UriKind.Absolute))
            {
                WriteMessageWithUsageToConsole("The provided snake absolute base URI is not a well-formed absolute URI string.");
                return false;
            }
            snakeMoveUri = new Uri(args[0] + "/move");
            if (snakeMoveUri.Scheme != Uri.UriSchemeHttp && snakeMoveUri.Scheme != Uri.UriSchemeHttps)
            {
                WriteMessageWithUsageToConsole("The provided snake absolute base URI scheme must be HTTP or HTTPS.");
                return false;
            }

            // Ensure that the second command line argument (the absolute path to the directory with the move assertion files) is well-formed.
            try
            {
                moveAssertionDirectory = Path.GetFullPath(args[1]);
            }
            catch
            {
                WriteMessageWithUsageToConsole("The provided absolute path to the directory with the move assertion files is not well-formed.");
                return false;
            }

            return true;
        }

        internal static bool GetMoveAssertionFilenameSegments(string moveAssertionFilename, ref string[] desiredDirections, ref string badDirection, ref string gameId)
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

            // Do not validate the third argument (it can be anything, preferbaly a GUID that is linked to the game).
            gameId = moveAssertionFilenameSegments[2];

            return true;
        }
        #endregion
    }
}
