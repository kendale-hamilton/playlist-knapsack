using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models.Knapsack;
using Models.Requests.Knapsack;
using Models.Routes;
using Models.ServiceResponse;
using Services.KnapsackService;
using Services.SupabaseService;

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
        public async Task<IActionResult> SolvePlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConstants.CustomPlaylists)] HttpRequestData req, string userId)
        {
            try {
                Console.WriteLine("Solving Playlist for Supabase user: " + userId);
            
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                SolvePlaylistRequest? body = JsonSerializer.Deserialize<SolvePlaylistRequest>(requestBody);
                DesiredLengths lengths = body.DesiredLengths;
                List<Track> tracks = body.Tracks;
                var solveRes = await _knapsackService.SolveKnapsack(lengths, tracks, userId);
                var customId = solveRes.Data;
                Console.WriteLine("Custom ID: " + customId);
                return ServiceResponse.ToIActionResult(solveRes);
            } catch (Exception ex) {
                Console.WriteLine("Error Solving Playlist: " + ex.Message);
                var res = new ServiceResponse<List<Track>>
                {
                    Status = System.Net.HttpStatusCode.InternalServerError,
                    ErrorMessage = ex.Message
                };
                return ServiceResponse.ToIActionResult(res);
            }
        }
        [Function("KnapsackGetCustomPlaylist")]
        public async Task<IActionResult> GetSolvedPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.CustomPlaylist)] HttpRequestData req, string userId, string customId)
        {
            Console.WriteLine("Getting Solved Playlist for Supabase user: " + userId);
            
            var res = await _knapsackService.GetCustomPlaylist(customId);
            return ServiceResponse.ToIActionResult(res);
        }

        [Function("GetCustomPlaylists")]
        public async Task<IActionResult> GetCustomPlaylists([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.CustomPlaylists)] HttpRequestData req, string userId)
        {
            Console.WriteLine("Getting Custom Playlists for Supabase user: " + userId);

            var res = await _knapsackService.GetCustomPlaylists(userId);
            return ServiceResponse.ToIActionResult(res);
        }

        [Function("KnapsackDeleteCustomPlaylist")]
        public async Task<IActionResult> DeleteCustomPlaylist([
            HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RouteConstants.CustomPlaylist)
        ] HttpRequestData req, string userId, string customId)
        {
            Console.WriteLine($"Deleting Custom Playlist {customId} for Supabase user: {userId}");
            var res = await _knapsackService.DeleteCustomPlaylist(customId);
            return ServiceResponse.ToIActionResult(res);
        }
    }
}