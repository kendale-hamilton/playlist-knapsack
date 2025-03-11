
using Models.Knapsack;
using Models.Requests.Knapsack;

namespace Services.KnapsackService
{
    public interface IKnapsackService
    {
        Task<string> SolveKnapsack(DesiredLengths desiredLengths, List<Track> tracks, string userId);
        Task<List<Track>> GetSolvedPlaylist(string userId, string filename);
    }
}