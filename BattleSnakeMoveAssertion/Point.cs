using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion
{
    internal sealed class Point
    {
        [JsonProperty(PropertyName = "object")]
        internal readonly string Object = "point";

        [JsonProperty(PropertyName = "x")]
        internal readonly int X;

        [JsonProperty(PropertyName = "y")]
        internal readonly int Y;
    }
}
