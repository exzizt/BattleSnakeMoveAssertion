# BattlesnakeMoveAssertion
This is a simple console app built using .NET Core that is used to assert that a snake makes your desired move(s) for each move request JSON file found in a given directory. Runs on Windows, Mac, and Linux. You just need .NET Core, which can be downloaded here: https://dotnet.microsoft.com/download.<br />
<br />
<b>Requirements</b>: .NET Core 2.0 or above<br />
<br />
<b>Usage</b><br />
```dotnet BattleSnakeMoveAssertion.dll <snake base URI> <path to directory with move assertion files>```<br />
<b>Example</b><br />
```dotnet BattleSnakeMoveAssertion.dll http://snakeai.com/api C:\Users\Bob\WorldJSON```<br />
<br />
<b>Move assertion filename format</b><br />
```<desired direction(s) separated by ->_<bad direction your snake moved>_<anything (e.g., a GUID)>.json```<br />
<b>Example 1</b><br />
```up_down_d8edf99a-21s-4326b-915b-d457eb49af6b.move```<br />
This means you want your snake to move UP instead of going DOWN.
<br />
<b>Example 2</b><br />
```left-right_up_x.move```<br />
This means you want your snake to go LEFT or RIGHT instead of going UP.
