using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion.Models
{
    internal sealed class Point
    {
        [JsonProperty(PropertyName = "x")]
        internal readonly int X;

        [JsonProperty(PropertyName = "y")]
        internal readonly int Y;
    }
}
