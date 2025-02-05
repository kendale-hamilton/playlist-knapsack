using System.Text.Json.Serialization;
using Models.Knapsack;

namespace Models.Spotify
{
    #region General
    public class SpotifyExternalUrls
    {
        [JsonPropertyName("spotify")]
        public string? Spotify { get; set; }
    }
    public class SpotifyImage
    {
        [JsonPropertyName("height")]
        public int? Height { get; set; }
        [JsonPropertyName("width")]
        public int? Width { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
    public class SpotifyVideoThumbnail
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
    public class SpotifyAlbum
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
        public List<SpotifyImage>? Images { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }
        [JsonPropertyName("release_date_precision")]
        public string? ReleaseDatePrecision { get; set; }
        [JsonPropertyName("uri")]
        public string? uri { get; set; }
        [JsonPropertyName("artists")]
        public List<SpotifyArtist>? Artists { get; set; }
        [JsonPropertyName("external_urls")]
        public SpotifyExternalUrls? ExternalUrls { get; set; }
        [JsonPropertyName("total_tracks")]
        public int? TotalTracks { get; set; }
    }
    public class SpotifyArtist
    {
        [JsonPropertyName("external_urls")]
        public SpotifyExternalUrls? ExternalUrls { get; set; }
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
    public class SpotifyExternalIds
    {
        [JsonPropertyName("isrc")]
        public string? Isrc { get; set; }
    }
    #endregion

    #region User
    public class SpotifyUser
    {
        [JsonPropertyName("external_urls")]
        public SpotifyExternalUrls? ExternalUrls { get; set; }
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("uri")]
        public string? Uri { get; set; }
    }
    public class SpotifyUserPlaylists
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
        public List<SpotifyPlaylistsItem>? Items { get; set; }
    }
    public class SpotifyPlaylistsItem
    {
        [JsonPropertyName("collaborative")]
        public bool? Collaborative { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("external_urls")]
        public SpotifyExternalUrls? ExternalUrls { get; set; }
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("images")]
        public List<SpotifyImage>? Images { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("owner")]
        public SpotifyUser? Owner { get; set; }
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
        public PlaylistDetails Simplify()
        {
            return new PlaylistDetails
            {
                Description = Description,
                Id = Id,
                Images = Images,
                Name = Name
            };
        }
    }
    #endregion

    #region Playlist
    public class SpotifyPlaylistItems
    {
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("items")]
        public List<SpotifyPlaylistTrack>? Items { get; set; }
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
    public class SpotifyPlaylistTrack
    {
        [JsonPropertyName("added_at")]
        public string? AddedAt { get; set; }
        [JsonPropertyName("added_by")]
        public SpotifyUser? AddedBy { get; set; }
        [JsonPropertyName("is_local")]
        public bool? IsLocal { get; set; }
        [JsonPropertyName("primary_color")]
        public string? PrimaryColor { get; set; }
        [JsonPropertyName("track")]
        public SpotifyTrack? Track { get; set; }
        [JsonPropertyName("video_thumbnail")]
        public SpotifyVideoThumbnail? VideoThumbnail { get; set; }
    }
    public class SpotifyTrack
    {
        [JsonPropertyName("preview_url")]
        public string? PreviewUrl { get; set; }
        [JsonPropertyName("available_markets")]
        public List<string>? AvailableMarkets { get; set; }
        [JsonPropertyName("explicit")]
        public bool? Explicit { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("episode")]
        public bool? Episode { get; set; }
        [JsonPropertyName("track")]
        public bool? IsTrack { get; set; }
        [JsonPropertyName("album")]
        public SpotifyAlbum? Album { get; set; }
        [JsonPropertyName("artists")]
        public List<SpotifyArtist>? Artists { get; set; }
        [JsonPropertyName("disc_number")]
        public int? DiscNumber { get; set; }
        [JsonPropertyName("track_number")]
        public int? TrackNumber { get; set; }
        [JsonPropertyName("duration_ms")]
        public int? DurationMs { get; set; }
        [JsonPropertyName("external_ids")]
        public SpotifyExternalIds? ExternalIds { get; set; }
        [JsonPropertyName("external_urls")]
        public SpotifyExternalUrls? ExternalUrls { get; set; }
        [JsonPropertyName("href")]
        public string? Href { get; set; }
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }
        [JsonPropertyName("uri")]
        public string? Uri { get; set; }
        [JsonPropertyName("is_local")]
        public bool IsLocal { get; set; }
        public Track Simplify()
        {
            return new Track
            {
                Album = Album,
                Artists = Artists,
                DurationMs = DurationMs,
                Name = Name,
                Popularity = Popularity
            };
        }
    }


    #endregion
}