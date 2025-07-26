namespace Models.Routes
{
    public static class RouteConstants
    {
        #region Spotify
        public const string SpotifyCallback = "spotify/callback";
        public const string SpotifyUserPlaylists = "spotify/users/{supaUserId}/playlists";
        public const string SpotifyPlaylist = "spotify/users/{supaUserId}/playlists/{playlistId}";
        public const string SpotifyDisconnect = "spotify/users/{supaUserId}/disconnect";
        #endregion
        #region Knapsack
        public const string CustomPlaylists = "knapsack/users/{supaUserId}/playlists";
        public const string CustomPlaylist = "knapsack/users/{supaUserId}/playlists/{customId}";
        #endregion
    }
}