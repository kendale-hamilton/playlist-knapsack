using System.Text.Json.Serialization;
using Models.Knapsack;

namespace Models.Requests.Knapsack
{
    public class SolvePlaylistRequest
    {
        [JsonPropertyName("desiredLengths")]
        public required DesiredLengths DesiredLengths { get; set; }
        [JsonPropertyName("tracks")]
        public required List<Track> Tracks { get; set; }
    }
    public class DesiredLengths
    {
        [JsonPropertyName("length")]
        public required int Length { get; set; }
        [JsonPropertyName("max")]
        public int? Max { get; set; }
        [JsonPropertyName("min")]
        public int? Min { get; set; }
    }
}