using Models.Knapsack;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Models.Supabase
{
    [Table("users")]
    public class UserRecord : BaseModel
    {
        [PrimaryKey("id")]
        public string? Id { get; set; }
        [Column("spotify_user_id")]
        public string? SpotifyUserId { get; set; }
        [Column("spotify_access_token")]
        public string? SpotifyAccessToken { get; set; }
        [Column("spotify_refresh_token")]
        public string? SpotifyRefreshToken { get; set; }
    }

    [Table("custom_playlists")]
    public class CustomPlaylistRecord : BaseModel
    {
        [PrimaryKey("id")]
        public string? Id { get; set; }
        [Column("user_id")]
        public string? UserId { get; set; }
        [Column("spotify_id")]
        public string? SpotifyId { get; set; }
        [Column("spotify_url")]
        public string? SpotifyUrl { get; set; }
        [Column("image_url")]
        public string? ImageUrl { get; set; }
        [Column("name")]
        public string? Name { get; set; }

    }

    [Table("playlist_tracks")]
    public class PlaylistTrackRecord : BaseModel
    {
        [PrimaryKey("id")]
        public string? Id { get; set; }
        [Column("playlist_id")]
        public string? PlaylistId { get; set; }
        [Column("track_id")]
        public string? TrackId { get; set; }
    }

    [Table("tracks")]
    public class TrackRecord : BaseModel
    {
        [PrimaryKey("id")]
        public string? Id { get; set; }
        [Column("spotify_id")]
        public string? SpotifyId { get; set; }
        [Column("seconds")]
        public int Seconds { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("spotify_url")]
        public string? SpotifyUrl { get; set; }
        [Column("uri")]
        public string? Uri { get; set; }
        [Column("artists_id")]
        public string? ArtistsId { get; set; }
        [Column("album_id")]
        public string? AlbumId { get; set; }
    }
}