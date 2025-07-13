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
        private readonly ISupabaseService _supabaseService;
        
        public KnapsackController(IKnapsackService knapsackService, ISupabaseService supabaseService)
        {
            _knapsackService = knapsackService;
            _supabaseService = supabaseService;
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
                return new OkObjectResult(new { customId });
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
        [Function("KnapsackGetSolvedPlaylist")]
        public async Task<IActionResult> GetSolvedPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.CustomPlaylist)] HttpRequestData req, string userId, string customId)
        {
            Console.WriteLine("Getting Solved Playlist for Supabase user: " + userId);
            
            var res = await _knapsackService.GetCustomPlaylist(customId);
            return ServiceResponse.ToIActionResult(res);
        }
    }
}