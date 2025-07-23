
using Models.Knapsack;
using Models.Requests.Knapsack;
using Models.ServiceResponse;

namespace Services.KnapsackService
{
    public interface IKnapsackService
    {
        Task<ServiceResponse<string>> SolveKnapsack(DesiredLengths desiredLengths, List<Track> tracks, string userId);
        Task<ServiceResponse<CustomPlaylist>> GetCustomPlaylist(string customId);
        Task<ServiceResponse<List<CustomPlaylistDetails>>> GetCustomPlaylists(string userId);
        Task<ServiceResponse<bool>> DeleteCustomPlaylist(string playlistId);
    }
}