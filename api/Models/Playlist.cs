using System.Text.Json.Serialization;

namespace Models
{
    public class PlaylistItemsResponse
    {
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("items")]
        public List<PlaylistTrack>? Items { get; set; }
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }
        [JsonPropertyName("next")]
        public string? Next { get; set; }
        [JsonPropertyName("offset")]
        public int? Offset { get; set; }
        [JsonPropertyName("previous")]
        public object? Previous { get; set; }
        [JsonPropertyName("total")]
        public int? Total { get; set; }        
    }
    public class PlaylistTrack
    {
        [JsonPropertyName("added_at")]
        public string? AddedAt { get; set; }
        [JsonPropertyName("added_by")]
        public User? AddedBy { get; set; }
        [JsonPropertyName("is_local")]
        public bool? IsLocal { get; set; }
        [JsonPropertyName("primary_color")]
        public string? PrimaryColor { get; set; }
        [JsonPropertyName("track")]
        public Track? Track { get; set; }
        [JsonPropertyName("video_thumbnail")]
        public VideoThumbnail? VideoThumbnail { get; set; }
    }
}