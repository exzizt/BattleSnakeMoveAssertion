using Newtonsoft.Json;

namespace BattleSnakeMoveAssertion
{
    internal static class Messages
    {
        internal sealed class World
        {
            [JsonProperty(PropertyName = "object")]
            public readonly string Object = "world";

            [JsonProperty(PropertyName = "id")]
            public readonly int Id;

            [JsonProperty(PropertyName = "width")]
            public readonly int Width;

            [JsonProperty(PropertyName = "height")]
            public readonly int Height;

            [JsonProperty(PropertyName = "turn")]
            public readonly int Turn;

            [JsonProperty(PropertyName = "you")]
            public readonly Snake You;

            [JsonProperty(PropertyName = "snakes")]
            public readonly Api.List<Snake> Snakes;

            [JsonProperty(PropertyName = "food")]
            public readonly Api.List<Point> Food;
        }

        internal sealed class MoveResponse
        {
            [JsonProperty(PropertyName = "move")]
            public readonly string Move;

            [JsonProperty(PropertyName = "taunt")]
            public readonly string Taunt;

            public MoveResponse(string move, string taunt = null)
            {
                Move = move;
                Taunt = taunt;
            }
        }
    }
}
