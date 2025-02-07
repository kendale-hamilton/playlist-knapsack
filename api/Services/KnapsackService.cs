
using Models.Knapsack;

namespace Services.KnapsackService
{
    public class KnapsackService : IKnapsackService
    {
        public KnapsackService(){}
        public Task<List<Track>> SolveKnapsack(int length, List<Track> tracks)
        {
            Console.WriteLine("Made it to KnapsackService");
            throw new NotImplementedException();
        }
    }
}