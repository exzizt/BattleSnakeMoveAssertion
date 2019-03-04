using Newtonsoft.Json;
using System.Collections.Generic;

namespace BattleSnakeMoveAssertion.Models
{
    internal sealed class Board
    {
        [JsonProperty(PropertyName = "width")]
        public readonly int Width;

        [JsonProperty(PropertyName = "height")]
        public readonly int Height;

        [JsonProperty(PropertyName = "snakes")]
        public readonly List<Snake> Snakes;

        [JsonProperty(PropertyName = "food")]
        public readonly List<Point> Food;
    }
}
