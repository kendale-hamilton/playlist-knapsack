using Models;

namespace API.Services
{
    public interface IServiceBase
    {
        Task<List<SimpUserPlaylistItem>> GetUserPlaylists(string userId, string token);
        Task<PlaylistItems> FetchPlaylistTracks(string playlistId, string token);
    }
}