namespace Models.Routes
{
    public static class RouteConstants
    {
        #region Spotify
        public const string SpotifyCallback = "spotify/callback";
        public const string SpotifyUserPlaylists = "spotify/users/{userId}/playlists";
        public const string SpotifyPlaylist = "spotify/users/{userId}/playlists/{playlistId}";
        public const string SpotifyDisconnect = "spotify/users/{userId}/disconnect";
        #endregion
        #region Knapsack
        public const string CustomPlaylists = "knapsack/users/{userId}/playlists";
        public const string CustomPlaylist = "knapsack/users/{userId}/playlists/{customId}";
        #endregion
    }
}