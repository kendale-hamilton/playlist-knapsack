
using Models.Knapsack;

namespace Services.KnapsackService
{
    public interface IKnapsackService
    {
        List<Track> SolveKnapsack(int length, List<Track> tracks);
    }
}