using SpotifyAPI.Web;

namespace API.Services
{
    public class ServiceBase : IServiceBase
    {
        private static SpotifyClient _spotify;

        public ServiceBase(IConfiguration configuration)
        {
            var clientId = configuration["Spotify:ClientId"];
            var clientSecret = configuration["Spotify:ClientSecret"];
            var config = SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));

            _spotify = new SpotifyClient(config);
        }

        public async Task<Paging<FullPlaylist>> FetchPlaylists(string userId)
        {
            return await _spotify.Playlists.GetUsers(userId);

        }
        
        public async Task<Paging<PlaylistTrack<IPlayableItem>>> FetchPlaylistTracks(string playlistId)
        {
            return await _spotify.Playlists.GetItems(playlistId);
        }
    }
}