using System.Text.Json.Serialization;

namespace Models
{
    public class VideoThumbnail
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
    public class Album
    {
        [JsonPropertyName("available_markets")]
        public List<string>? AvailableMarkets { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("album_type")]
        public string? AlbumType { get; set; }
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("images")]
        public List<Image>? Images { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }
        [JsonPropertyName("release_date_precision")]
        public string? ReleaseDatePrecision { get; set; }
        [JsonPropertyName("uri")]
        public string? uri { get; set; }
        [JsonPropertyName("artists")]
        public List<Artist>? Artists { get; set; }
        [JsonPropertyName("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }
        [JsonPropertyName("total_tracks")]
        public int? TotalTracks { get; set; }
    }
    public class Artist
    {
        [JsonPropertyName("external_urls")]
        public ExternalUrls? ExternalUrls { get; set; }
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("uri")]
        public string? Uri { get; set; }
    }
    public class ExternalIds
    {
        [JsonPropertyName("isrc")]
        public string? Isrc { get; set; }
    }
    public class ExternalUrls
    {
        [JsonPropertyName("spotify")]
        public string? Spotify { get; set; }
    }
    public class Image
    {
        [JsonPropertyName("height")]
        public int? Height { get; set; }
        [JsonPropertyName("width")]
        public int? Width { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}