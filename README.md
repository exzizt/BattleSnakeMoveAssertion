# BattleSnakeMoveAssertion
Console app built in .NET Core 2.0 to assert a snake's moves for each world JSON file in a given directory. Runs on Windows, Max, and Linux.<br />
<br />
Requirements: .NET Core 2.0 and command line.<br />
<br />
Usage: dotnet BattleSnakeMoveAssertion.dll <snake base URI> <path to directory with move assertion files><br />
Example: dotnet BattleSnakeMoveAssertion.dll http://snakeai.com/api C:\Users\Bob\WorldJSON<br />
<br />
Move assertion file format: <desired direction(s) separated by ->&#95;<bad direction your snake moved>&#95;<anything (e.g., SnakeDown game ID)>.move<br />
Example 1: up_down_d8edf99a-21s-4326b-915b-d457eb49af6b.move<br />
Example 2: left-right_up_x.move
