using System.Collections.ObjectModel;

namespace BattleSnakeMoveAssertion.Models
{
    internal static class Api
    {
        internal static class Move
        {
            internal const string Up = "up";
            internal const string Down = "down";
            internal const string Left = "left";
            internal const string Right = "right";
            internal static readonly ReadOnlyCollection<string> All = new ReadOnlyCollection<string>(new[] { Up, Down, Left, Right });
        }
    }
}
