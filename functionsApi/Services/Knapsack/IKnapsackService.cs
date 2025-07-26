
using Models.Knapsack;
using Models.Requests.Knapsack;
using Models.ServiceResponse;
using Models.Supabase;
using Services.Base;

namespace Services.KnapsackService
{
    public interface IKnapsackService
    {
        Task<ServiceResponse<string>> SolveKnapsack(DesiredLengths desiredLengths, List<Track> tracks, string userId);
        Task<ServiceResponse<CustomPlaylist>> GetCustomPlaylist(string customId);
        Task<ServiceResponse<List<CustomPlaylistDetails>>> GetCustomPlaylists(string userId);
        Task<ServiceResponse<bool>> UpdateCustomPlaylist(CustomPlaylistRecord playlist);
        Task<ServiceResponse<CustomPlaylistRecord>> DeleteCustomPlaylist(string playlistId);
    }
}