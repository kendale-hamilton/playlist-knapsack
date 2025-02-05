using System.Text.Json.Serialization;
using Models.Spotify;

namespace Models.Knapsack
{
    #region Playliste
    public class Playlist
    {
        [JsonPropertyName("details")]
        public PlaylistDetails Details { get; set; }
        [JsonPropertyName("tracks")]
        public List<Track> Tracks { get; set; }
    }

    public class PlaylistDetails
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("images")]
        public List<SpotifyImage>? Images { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; } 
        [JsonPropertyName("duration_ms")]
        public int DurationMs { get; set; }
    }
    #endregion
    #region Track
    public class Track
    {
        [JsonPropertyName("album")]
        public SpotifyAlbum? Album { get; set; }
        [JsonPropertyName("artists")]
        public List<SpotifyArtist>? Artists { get; set; }
        [JsonPropertyName("duration_ms")]
        public int? DurationMs { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }
    }
    #endregion
}