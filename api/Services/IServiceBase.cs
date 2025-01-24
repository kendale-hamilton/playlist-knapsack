using Models;

namespace API.Services
{
    public interface IServiceBase
    {
        Task<List<SimpUserPlaylistItem>> GetUserPlaylists(string userId, string token);
        Task<SimpPlaylist> FetchPlaylist(string playlistId, string token);
        Task<List<SimpTrack>> FetchPlaylistTracks(string playlistId, string token);
    }
}