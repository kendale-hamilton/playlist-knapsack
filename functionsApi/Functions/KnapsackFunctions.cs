using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models.Knapsack;
using Models.Requests.Knapsack;
using Models.Routes;
using Services.KnapsackService;
using Services.SupabaseService;

namespace Controllers.KnapsackController
{
    public class KnapsackController : ControllerBase
    {
        private readonly IKnapsackService _knapsackService;
        private readonly ISupabaseService _supabaseService;
        
        public KnapsackController(IKnapsackService knapsackService, ISupabaseService supabaseService)
        {
            _knapsackService = knapsackService;
            _supabaseService = supabaseService;
        }
        [Function("KnapsackSolvePlaylist")]
        public async Task<IActionResult> SolvePlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConstants.CustomPlaylists)] HttpRequestData req, string userId)
        {
            Console.WriteLine("Solving Playlist for Supabase user: " + userId);
            
            // Get Spotify user ID from Supabase
            var spotifyUserIdResponse = await _supabaseService.GetSpotifyUserId(userId);
            if (spotifyUserIdResponse.Status != System.Net.HttpStatusCode.OK)
            {
                return NotFound("User not found or Spotify not connected");
            }
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SolvePlaylistRequest? body = JsonSerializer.Deserialize<SolvePlaylistRequest>(requestBody);
            DesiredLengths lengths = body.DesiredLengths;
            List<Track> tracks = body.Tracks;
            string id = await _knapsackService.SolveKnapsack(lengths, tracks, spotifyUserIdResponse.Data);
            return Ok(new {id});
        }
        [Function("KnapsackGetSolvedPlaylist")]
        public async Task<IActionResult> GetSolvedPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.CustomPlaylist)] HttpRequestData req, string userId, string customId)
        {
            Console.WriteLine("Getting Solved Playlist for Supabase user: " + userId);
            
            // Get Spotify user ID from Supabase
            var spotifyUserIdResponse = await _supabaseService.GetSpotifyUserId(userId);
            if (spotifyUserIdResponse.Status != System.Net.HttpStatusCode.OK)
            {
                return NotFound("User not found or Spotify not connected");
            }
            
            List<Track> tracks = await _knapsackService.GetSolvedPlaylist(spotifyUserIdResponse.Data, customId);
            return Ok(tracks);
        }
    }
}