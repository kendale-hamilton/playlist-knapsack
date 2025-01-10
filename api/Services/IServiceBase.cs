using Models;

namespace API.Services
{
    public interface IServiceBase
    {

        Task<string> FetchPlaylists(string userId, string token);
        Task<PlaylistItemsResponse> FetchPlaylistTracks(string playlistId, string token);
    }
}