using System.Text.Json.Serialization;
using Models.Knapsack;

namespace Models.Requests.Spotify
{
    public class SpotifyPostPlaylistRequest
    {
        [JsonPropertyName("playlist")]
        public required Playlist Playlist { get; set; }
        // [JsonPropertyName("image")]
        // public string? Image { get; set; }
    }
}