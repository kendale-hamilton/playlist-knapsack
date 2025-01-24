using System.Text.Json.Serialization;

namespace Models
{
    public class User
    {
        [JsonPropertyName("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("uri")]
        public string? Uri { get; set; }
    }

    public class UserPlaylists
    {
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }
        [JsonPropertyName("next")]
        public string? Next { get; set; }
        [JsonPropertyName("offset")]
        public int? Offset { get; set; }
        [JsonPropertyName("previous")]
        public string? Previous { get; set; }
        [JsonPropertyName("total")]
        public int? Total { get; set; }
        [JsonPropertyName("items")]
        public List<PlaylistsItem>? Items { get; set; }
    }

    public class PlaylistsItem
    {
        [JsonPropertyName("collaborative")]
        public bool? Collaborative { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("images")]
        public List<Image>? Images { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("owner")]
        public User? Owner { get; set; }
        [JsonPropertyName("primary_color")]
        public string? PrimaryColor { get; set; }
        [JsonPropertyName("public")]
        public bool? Public { get; set; }
        [JsonPropertyName("snapshot_id")]
        public string? SnapshotId { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("uri")]
        public string? Uri { get; set; }
        public SimpUserPlaylistItem Simplify()
        {
            return new SimpUserPlaylistItem
            {
                Description = Description,
                Id = Id,
                Images = Images,
                Name = Name
            };
        }
    }

    public class SimpUserPlaylistItem
    {

        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("images")]
        public List<Image>? Images { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; } 
    }
}