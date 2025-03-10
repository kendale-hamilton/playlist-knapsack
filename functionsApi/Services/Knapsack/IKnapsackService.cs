
using Models.Knapsack;

namespace Services.KnapsackService
{
    public interface IKnapsackService
    {
        Task<string> SolveKnapsack(int length, List<Track> tracks, string userId);
        Task<List<Track>> GetSolvedPlaylist(string userId, string filename);
    }
}