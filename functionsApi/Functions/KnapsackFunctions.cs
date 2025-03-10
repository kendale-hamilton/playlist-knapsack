using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models.Knapsack;
using Models.Requests.Knapsack;
using Models.Routes;
using Services.KnapsackService;

namespace Controllers.KnapsackController
{
    public class KnapsackController : ControllerBase
    {
        private readonly IKnapsackService _knapsackService;
        public KnapsackController(IKnapsackService knapsackService)
        {
            _knapsackService = knapsackService;
        }
        [Function("KnapsackSolvePlaylist")]
        public async Task<IActionResult> SolvePlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConstants.CustomPlaylists)] HttpRequestData req, string spotifyUserId)
        {
            Console.WriteLine("Solving Playlist...");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SolvePlaylistRequest? body = JsonSerializer.Deserialize<SolvePlaylistRequest>(requestBody);
            int length = body.Length;
            List<Track> tracks = body.Tracks;
            string id = await _knapsackService.SolveKnapsack(length, tracks, spotifyUserId);
            return Ok(new {id});
        }
        [Function("KnapsackGetSolvedPlaylist")]
        public async Task<IActionResult> GetSolvedPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.CustomPlaylist)] HttpRequestData req, string spotifyUserId, string customId)
        {
            Console.WriteLine("Getting Solved Playlist...");
            List<Track> tracks = await _knapsackService.GetSolvedPlaylist(spotifyUserId, customId);
            return Ok(tracks);
        }
    }
}