using Models.Knapsack;

namespace Models.Requests.Knapsack
{
    public class PostKnapsackSolverRequest
    {
        public required int Length { get; set; }
        public required List<Track> Tracks { get; set; }
        public required string UserId { get; set; }
    }
}