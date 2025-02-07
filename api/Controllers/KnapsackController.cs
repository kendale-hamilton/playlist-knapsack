using Microsoft.AspNetCore.Mvc;
using Models.Knapsack;
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
        [HttpPost("playlist")]
        public async Task<IActionResult> SolvePlaylist([FromQuery] int length, [FromBody] List<Track> tracks)
        {
            Console.WriteLine("Solving Playlist...");
            List<Track> playlist = await _knapsackService.SolveKnapsack(length, tracks);
            return Ok(playlist);
        }
    }
}