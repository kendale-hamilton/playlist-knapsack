using Microsoft.AspNetCore.Mvc;
using Models.Knapsack;
using Models.Requests.Knapsack;
using Services.KnapsackService;

namespace Controllers.KnapsackController
{
    [ApiController]
    [Route("api/knapsack")]
    public class KnapsackController : ControllerBase
    {
        private readonly IKnapsackService _knapsackService;
        public KnapsackController(IKnapsackService knapsackService)
        {
            _knapsackService = knapsackService;
        }
        [HttpPost("playlists")]
        public async Task<IActionResult> SolvePlaylist([FromBody] PostKnapsackSolverRequest request)
        {
            Console.WriteLine("Solving Playlist...");
            int length = request.Length;
            List<Track> tracks = request.Tracks;
            string userId = request.UserId;
            string id = await _knapsackService.SolveKnapsack(length, tracks, userId);
            return Ok(new {id});
        }
        [HttpGet("playlists/{userId}/{customId}")]
        public async Task<IActionResult> GetSolvedPlaylist(string userId, string customId)
        {
            Console.WriteLine("Getting Solved Playlist...");
            List<Track> tracks = await _knapsackService.GetSolvedPlaylist(userId, customId);
            return Ok(tracks);
        }
    }
}