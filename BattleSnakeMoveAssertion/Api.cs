using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion
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

        internal class List<T>
        {
            [JsonProperty(PropertyName = "object")]
            internal readonly string Object = "list";

            [JsonProperty(PropertyName = "data")]
            internal T[] Data;

            public List(T[] data)
            {
                Data = data;
            }
        }
    }
}
