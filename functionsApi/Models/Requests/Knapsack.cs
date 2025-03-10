using System.Text.Json.Serialization;
using Models.Knapsack;

namespace Models.Requests.Knapsack
{
    public class SolvePlaylistRequest
    {
        [JsonPropertyName("length")]
        public required int Length { get; set; }
        [JsonPropertyName("tracks")]
        public required List<Track> Tracks { get; set; }
    }
}