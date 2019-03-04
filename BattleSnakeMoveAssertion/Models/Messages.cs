using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion.Models
{
    internal static class Messages
    {
        internal sealed class MoveRequest
        {
            [JsonProperty(PropertyName = "game")]
            public readonly Game Game;

            [JsonProperty(PropertyName = "turn")]
            public readonly int Turn;

            [JsonProperty(PropertyName = "board")]
            public readonly Board Board;

            [JsonProperty(PropertyName = "you")]
            public readonly Snake You;
        }

        internal sealed class MoveResponse
        {
            [JsonProperty(PropertyName = "move")]
            internal readonly string Move;
        }
    }
}
