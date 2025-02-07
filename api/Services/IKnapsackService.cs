
using Models.Knapsack;

namespace Services.KnapsackService
{
    public interface IKnapsackService
    {
        Task<List<Track>> SolveKnapsack(int length, List<Track> tracks);
    }
}