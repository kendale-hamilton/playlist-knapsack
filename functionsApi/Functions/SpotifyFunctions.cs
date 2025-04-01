
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models.Knapsack;
using Models.Requests.Spotify;
using Models.Routes;
using Models.ServiceResponse;
using Services.SpotifyService;

namespace Controllers.SpotifyController
{
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;
        public SpotifyController(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }
        [Function("SpotifyRefreshToken")]
        public async Task<HttpResponseData> RefreshToken([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyRefreshToken)] HttpRequestData req, string refreshToken)
        {
            Console.WriteLine("Refreshing Token..." + refreshToken);
            string accessToken = await _spotifyService.RefreshAccessToken(refreshToken);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Cookies.Append("accessToken", accessToken);
            return response;
        }
        [Function("SpotifyGetUserPlaylists")]
        public async Task<IActionResult> GetUserPlaylists([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyUserPlaylists)] HttpRequestData req, string userId)
        {
            Console.WriteLine("Getting User Playlists...");
            var accessToken = req.Headers.GetValues("Authorization").FirstOrDefault().Replace("Bearer ", "");
            ServiceResponse<List<PlaylistDetails>> userPlaylistsResponse = await _spotifyService.GetUserPlaylists(userId, accessToken);
            Console.WriteLine(userPlaylistsResponse.Status);
            return userPlaylistsResponse.ToActionResult();
        }
        [Function("SpotifyGetPlaylist")]
        public async Task<ActionResult> GetPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyPlaylist)] HttpRequestData req, string playlistId)
        {
            Console.WriteLine("Getting Playlist Tracks...");
            var accessToken = req.Headers.GetValues("Authorization").FirstOrDefault().Replace("Bearer ", "");
            ServiceResponse<PlaylistDetails> detailsResponse = await _spotifyService.GetPlaylistDetails(playlistId, accessToken);
            ServiceResponse<List<Track>> playlistTracksResponse = await _spotifyService.GetPlaylistTracks(playlistId, accessToken);
            if (detailsResponse.Status == HttpStatusCode.Unauthorized || playlistTracksResponse.Status == HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            else if (detailsResponse.Status != HttpStatusCode.OK || playlistTracksResponse.Status != HttpStatusCode.OK)
            {
                return BadRequest();
            }
            PlaylistDetails details = detailsResponse.Data;
            List<Track> playlistTracks = playlistTracksResponse.Data;
            int duration = 0;
            foreach (Track t in playlistTracks)
            {
                duration += t.Seconds;
            }
            details.Seconds = duration;
            Playlist playlistInfo= new Playlist
            {
                Details = details,
                Tracks = playlistTracks
            };
            return Ok(playlistInfo);
        }
        [Function("SpotifyPostPlaylist")]
        public async Task<IActionResult> PostPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConstants.SpotifyUserPlaylists)] HttpRequestData req, string userId)
        {
            Console.WriteLine("Creating Playlist...");
            var accessToken = req.Headers.GetValues("Authorization").FirstOrDefault().Replace("Bearer ", "");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SpotifyPostPlaylistRequest? body = JsonSerializer.Deserialize<SpotifyPostPlaylistRequest>(requestBody);
            Playlist playlist = body.Playlist;
            // string image = body.Image;
            ServiceResponse<string> urlResponse = await _spotifyService.UploadPlaylist(userId, playlist, accessToken);
            if (urlResponse.Status == HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            else if (urlResponse.Status != HttpStatusCode.OK)
            {
                return BadRequest();
            }
            return urlResponse.ToActionResult();
        }
    }
}