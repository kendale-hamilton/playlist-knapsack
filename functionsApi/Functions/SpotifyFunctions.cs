
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
        [Function("SpotifyCallback")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyCallback)] HttpRequestData req)
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
            Console.WriteLine("Client ID: " + clientId);
            Console.WriteLine("Client Secret: " + clientSecret);
            Console.WriteLine("Redirect URI: " + redirectUri);

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
                Console.WriteLine($"Token content: {tokenContent}");
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

                string redirect_uri = Environment.GetEnvironmentVariable("FRONTEND_REDIRECT_URI");
                response.Headers.Add("Location", redirect_uri);
                return response;
                
            }
        }

        //Expired accessToken: BQCbXlxbX9dUYebc1Dt7s5DPe94ymaVIoiDNyW1Fh8gVutdSLbom2gotqzW0Vj5EsoeuRhE1HnwwCdX1R0Kcw-Sr_UH76WCyM8IAUEHiMzv7Gtr8B_j4KQTnMRzEcHWXgrMJX8PPQZ4izVg6MynFGZbdeL6itu-k64NmhLQdgs_NyUeqKbecbByMdhcRJyfLoiiyBSCUsqy6PPhKe7hd-Bmg0vtl0V2T_d5r37bM5urmEizJeSOZZarj9YUw1m-5s_Yz3Sb1cEiPshxNW1TlWs1G-PYzxNO8ao7whwEO9-vvwYDkhNt_F_zTL-xJ
        //Refresh token: AQAUhZBOcl-q7egJ0OYZDz8yatVZbHZIjmtzN1KzsljdR1M5vEQFAWIXERu55j_XrYwoqD6V6nk0PPwMwHDbK0dbbd6KRHOQMLX_GsJVqduQFDtfbZ_TV-0c1Cy-gZ6Qqd8
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