using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion
{
    internal sealed class Snake
    {
        [JsonProperty(PropertyName = "object")]
        internal readonly string Object = "snake";

        [JsonProperty(PropertyName = "id")]
        internal readonly string Id;

        [JsonProperty(PropertyName = "name")]
        internal readonly string Name;

        [JsonProperty(PropertyName = "taunt")]
        internal readonly string Taunt;

        [JsonProperty(PropertyName = "health")]
        internal readonly int Health;

        [JsonProperty(PropertyName = "body")]
        internal readonly Api.List<Point> Body;

        [JsonProperty(PropertyName = "length")]
        internal readonly int Length;
    }
}
