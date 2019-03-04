using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion.Models
{
    internal sealed class Snake
    {
        [JsonProperty(PropertyName = "id")]
        internal readonly string Id;

        [JsonProperty(PropertyName = "name")]
        internal readonly string Name;

        [JsonProperty(PropertyName = "health")]
        internal readonly int Health;

        [JsonProperty(PropertyName = "body")]
        internal readonly Point[] Body;
    }
}
