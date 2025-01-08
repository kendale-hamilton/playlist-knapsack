using SpotifyAPI.Web;

namespace API.Services
{
    public class PlaylistService : ServiceBase, IPlaylistService
    {
        public PlaylistService(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task<Paging<FullPlaylist>> GetPlaylists(string userId)
        {
            Console.WriteLine("Getting playlists...");
            return await FetchPlaylists(userId);
        }
        public async Task<Paging<PlaylistTrack<IPlayableItem>>> GetPlaylistTracks(string playlistId)
        {
            Console.WriteLine("Getting playlist tracks...");
            return await FetchPlaylistTracks(playlistId);
        }
    }
}