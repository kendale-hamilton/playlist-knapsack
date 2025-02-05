using Models.Knapsack;

namespace Services
{
    public interface IServiceBase
    {
        Task<List<PlaylistDetails>> GetUserPlaylists(string userId, string token);
        Task<PlaylistDetails> GetPlaylistDetails(string playlistId, string token);
        Task<List<Track>> GetPlaylistTracks(string playlistId, string token);
    }
}