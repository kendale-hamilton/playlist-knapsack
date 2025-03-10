
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models.Knapsack;
using Models.Routes;
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
        [Function("SpotifyCallback")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyCallback)] HttpRequestData req)
        {
            Console.WriteLine("Callback...");
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var code = query["code"];
            var state = query["state"];

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await errorResponse.WriteStringAsync("Missing code or state parameter.");
                return errorResponse;
            }

            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
            var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI");

            using (var client = new HttpClient())
            {
                var tokenResponse = await client.PostAsync("https://accounts.spotify.com/api/token", new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                }));

                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                var tokenJson = JsonObject.Parse(tokenContent);
                var accessToken = tokenJson["access_token"]?.ToString();
                var refreshToken = tokenJson["refresh_token"]?.ToString();

                if (string.IsNullOrEmpty(accessToken))
                {
                    var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                    await errorResponse.WriteStringAsync("Failed to get access token.");
                    return errorResponse;
                }
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var meResponse = await client.GetAsync("https://api.spotify.com/v1/me");

                var meContent = await meResponse.Content.ReadAsStringAsync();
                var meJson = JsonObject.Parse(meContent);
                var userId = meJson["id"]?.ToString();
                var displayName = meJson["display_name"]?.ToString();
                var email = meJson["email"]?.ToString();
                var avatar = meJson["images"]?[0]?["url"]?.ToString();

                var response = req.CreateResponse(System.Net.HttpStatusCode.Redirect);
                response.Cookies.Append("userId", userId);
                response.Cookies.Append("userDisplayName", displayName);
                response.Cookies.Append("userEmail", email);
                response.Cookies.Append("userAvatar", avatar);
                response.Cookies.Append("accessToken", accessToken);
                response.Cookies.Append("refreshToken", refreshToken);
                response.Headers.Add("Location", "http://localhost:4280");
                return response;
                
            }
        }
        [Function("SpotifyGetUserPlaylists")]
        public async Task<IActionResult> GetUserPlaylists([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyUserPlaylists)] HttpRequestData req, string userId)
        {
            Console.WriteLine("Getting User Playlists...");
            var accessToken = req.Headers.GetValues("Authorization").FirstOrDefault().Replace("Bearer ", "");
            List<PlaylistDetails> userPlaylists = await _spotifyService.GetUserPlaylists(userId, accessToken);
            return Ok(userPlaylists);
        }
        [Function("SpotifyGetPlaylist")]
        public async Task<ActionResult> GetPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyPlaylist)] HttpRequestData req, string playlistId)
        {
            Console.WriteLine("Getting Playlist Tracks...");
            var accessToken = req.Headers.GetValues("Authorization").FirstOrDefault().Replace("Bearer ", "");
            PlaylistDetails details = await _spotifyService.GetPlaylistDetails(playlistId, accessToken);
            List<Track> playlistTracks = await _spotifyService.GetPlaylistTracks(playlistId, accessToken);
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
            Playlist? playlist = JsonSerializer.Deserialize<Playlist>(requestBody);
            string href = await _spotifyService.UploadPlaylist(userId, playlist, accessToken);
            return Ok(href);
        }
    }
}