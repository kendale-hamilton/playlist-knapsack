
namespace Models.Routes
{
    public static class RouteConstants
    {
        #region Spotify
        public const string SpotifyUserPlaylists = "spotify/users/{userId}/playlists";
        public const string SpotifyPlaylist = "spotify/playlists/{playlistId}";
        #endregion
        #region Knapsack
        public const string CustomPlaylists = "knapsack/users/{spotifyUserId}/playlists";
        public const string CustomPlaylist = "knapsack/users/{spotifyUserId}/playlists/{customId}";
        #endregion
    }
}