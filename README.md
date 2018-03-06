# BattleSnakeMoveAssertion
Console app built in .NET Core 2.0 to assert a snake's moves for each world JSON file in a given directory. Runs on Windows, Max, and Linux.<br />
<br />
<b>Requirements</b>: .NET Core 2.0 and command line.<br />
<br />
<b>Usage</b>: dotnet BattleSnakeMoveAssertion.dll <snake base URI> <path to directory with move assertion files><br />
<b>Example</b>: dotnet BattleSnakeMoveAssertion.dll http://snakeai.com/api C:\Users\Bob\WorldJSON<br />
<br />
<b>Move assertion file format</b>: ```<desired direction(s) separated by ->_<bad direction your snake moved>_<anything (e.g., SnakeDown game ID)>.move```<br />
<b>Example 1</b>: up_down_d8edf99a-21s-4326b-915b-d457eb49af6b.move<br />
<b>Example 2</b>: left-right_up_x.move
