using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion.Models
{
    internal sealed class Game
    {
        [JsonProperty(PropertyName = "id")]
        internal readonly string Id;
    }
}
