using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models.Knapsack;
using Models.Requests.Knapsack;
using Models.Routes;
using Models.ServiceResponse;
using Models.Supabase;
using Services.KnapsackService;
using Services.SpotifyService;

namespace Controllers.KnapsackController
{
    public class KnapsackController : ControllerBase
    {
        private readonly IKnapsackService _knapsackService;
        private readonly ISpotifyService _spotifyService;
        
        public KnapsackController(IKnapsackService knapsackService, ISpotifyService spotifyService)
        {
            _knapsackService = knapsackService;
            _spotifyService = spotifyService;
        }
        [Function("KnapsackSolvePlaylist")]
        public async Task<IActionResult> SolvePlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConstants.CustomPlaylists)] HttpRequestData req, string supaUserId)
        {
            try {
                Console.WriteLine("Solving Playlist for Supabase user: " + supaUserId);
            
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                SolvePlaylistRequest? body = JsonSerializer.Deserialize<SolvePlaylistRequest>(requestBody);
                DesiredLengths lengths = body.DesiredLengths;
                List<Track> tracks = body.Tracks;
                var solveRes = await _knapsackService.SolveKnapsack(lengths, tracks, supaUserId);
                if (solveRes.Status != HttpStatusCode.OK)
                {
                    return ServiceResponse.ToIActionResult(solveRes);
                }
                var uploadRes = await _knapsackService.CreateCustomPlaylist(solveRes.Data, supaUserId);
                if (uploadRes.Status != HttpStatusCode.OK)
                {
                    return ServiceResponse.ToIActionResult(uploadRes);
                }
                var idRes = new ServiceResponse<string>
                {
                    Status = HttpStatusCode.OK,
                    Data = uploadRes.Data.Id
                };
                return ServiceResponse.ToIActionResult(idRes);
            } catch (Exception ex) {
                Console.WriteLine("Error Solving Playlist: " + ex.Message);
                var res = new ServiceResponse<List<Track>>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = ex.Message
                };
                return ServiceResponse.ToIActionResult(res);
            }
        }
        [Function("KnapsackGetCustomPlaylist")]
        public async Task<IActionResult> GetSolvedPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.CustomPlaylist)] HttpRequestData req, string supaUserId, string customId)
        {
            Console.WriteLine("Getting Solved Playlist for Supabase user: " + supaUserId);
            
            var res = await _knapsackService.GetCustomPlaylist(customId);
            return ServiceResponse.ToIActionResult(res);
        }

        [Function("GetCustomPlaylists")]
        public async Task<IActionResult> GetCustomPlaylists([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.CustomPlaylists)] HttpRequestData req, string supaUserId)
        {
            Console.WriteLine("Getting Custom Playlists for Supabase user: " + supaUserId);

            var res = await _knapsackService.GetCustomPlaylists(supaUserId);
            if (res.Status != HttpStatusCode.OK)
            {
                return ServiceResponse.ToIActionResult(res);
            }
            var playlists = res.Data;
            var tokenRes = await _spotifyService.GetValidAccessToken(supaUserId);
            if (tokenRes.Status != HttpStatusCode.OK)
            {
                return ServiceResponse.ToIActionResult(tokenRes);
            }

            var playlistsWithImages = (await Task.WhenAll(playlists.Select(async p => 
            {
                if (p.SpotifyId != null && p.ImageUrl == null)
                {
                    var imageRes = await _spotifyService.GetPlaylistImage(p.SpotifyId, tokenRes.Data);
                    var updatedRecord = new CustomPlaylistRecord
                    {
                        Id = p.Id,
                        UserId = supaUserId,
                        SpotifyId = p.SpotifyId,
                        SpotifyUrl = p.SpotifyUrl,
                        ImageUrl = p.ImageUrl,
                        Name = p.Name
                    };
                    var updatedRes = await _knapsackService.UpdateCustomPlaylist(updatedRecord);
                    if (updatedRes.Status != HttpStatusCode.OK)
                    {
                        return p;
                    }
                    p.ImageUrl = imageRes.Data;
                    return p;
                }
                return p;
            }))).ToList();

            var serviceRes = new ServiceResponse<List<CustomPlaylistDetails>>
            {
                Status = HttpStatusCode.OK,
                Data = playlistsWithImages
            };
            return ServiceResponse.ToIActionResult(serviceRes);
        }

        [Function("KnapsackDeleteCustomPlaylist")]
        public async Task<IActionResult> DeleteCustomPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = RouteConstants.CustomPlaylist)] HttpRequestData req, string supaUserId, string customId)
        {
            Console.WriteLine($"Deleting Custom Playlist {customId} for Supabase user: {supaUserId}");
            var res = await _knapsackService.DeleteCustomPlaylist(customId);
            if (res.Status != HttpStatusCode.OK)
            {
                return ServiceResponse.ToIActionResult(res);
            }
            var record = res.Data;

            var tokenRes = await _spotifyService.GetValidAccessToken(supaUserId);

            if (record.SpotifyId != null)
            {
                var deleteRes = await _spotifyService.DeleteSpotifyPlaylist(record.SpotifyId, tokenRes.Data);
                if (deleteRes.Status != HttpStatusCode.OK)
                {
                    return ServiceResponse.ToIActionResult(deleteRes);
                }
            }

            return ServiceResponse.ToIActionResult(res);
        }
    }
}