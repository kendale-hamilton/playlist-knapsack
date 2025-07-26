using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Models.Knapsack;
using Models.Requests.Spotify;
using Models.Routes;
using Models.ServiceResponse;
using Services.SpotifyService;
using Services.SupabaseService;

namespace Controllers.SpotifyController
{
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;
        
        public SpotifyController(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [Function("SpotifyGetUserPlaylists")]
        public async Task<IActionResult> GetUserPlaylists([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyUserPlaylists)] HttpRequestData req, string supaUserId)
        {
            Console.WriteLine("Getting User Playlists for Supabase user: " + supaUserId);
            
            var res = await _spotifyService.GetSpotifyUserId(supaUserId);
            if (res.Status != HttpStatusCode.OK)
            {
                return NotFound($"Supabase connection failed: {res.ErrorMessage}");
            }
            
            var accessTokenResponse = await _spotifyService.GetValidAccessToken(supaUserId);
            if (accessTokenResponse.Status != HttpStatusCode.OK)
            {
                return Unauthorized(accessTokenResponse.ErrorMessage);
            }
            
            ServiceResponse<List<PlaylistDetails>> userPlaylistsResponse = await _spotifyService.GetUserPlaylists(res.Data, accessTokenResponse.Data);
            return ServiceResponse.ToIActionResult(userPlaylistsResponse);
        }
        
        [Function("SpotifyGetPlaylist")]
        public async Task<ActionResult> GetPlaylist([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = RouteConstants.SpotifyPlaylist)] HttpRequestData req, string userId, string playlistId)
        {
            Console.WriteLine("Getting Playlist Tracks for Supabase user: " + userId);
            
            var accessTokenResponse = await _spotifyService.GetValidAccessToken(userId);
            if (accessTokenResponse.Status != HttpStatusCode.OK)
            {
                return Unauthorized(accessTokenResponse.ErrorMessage);
            }
            
            ServiceResponse<PlaylistDetails> detailsResponse = await _spotifyService.GetPlaylistDetails(playlistId, accessTokenResponse.Data);
            ServiceResponse<List<Track>> playlistTracksResponse = await _spotifyService.GetPlaylistTracks(playlistId, accessTokenResponse.Data);
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
            Console.WriteLine("Creating Playlist for Supabase user: " + userId);
            
            var spotifyUserIdResponse = await _spotifyService.GetSpotifyUserId(userId);
            if (spotifyUserIdResponse.Status != HttpStatusCode.OK)
            {
                return NotFound("User not found or Spotify not connected");
            }
            
            var accessTokenResponse = await _spotifyService.GetValidAccessToken(userId);
            if (accessTokenResponse.Status != HttpStatusCode.OK)
            {
                return Unauthorized(accessTokenResponse.ErrorMessage);
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SpotifyPostPlaylistRequest? body = JsonSerializer.Deserialize<SpotifyPostPlaylistRequest>(requestBody);
            Playlist playlist = body.Playlist;
            ServiceResponse<string> urlResponse = await _spotifyService.UploadPlaylist(userId, spotifyUserIdResponse.Data, playlist, accessTokenResponse.Data);
            if (urlResponse.Status == HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            else if (urlResponse.Status != HttpStatusCode.OK)
            {
                return BadRequest();
            }
            return ServiceResponse.ToIActionResult(urlResponse);
        }

        [Function("DisconnectSpotify")]
        public async Task<IActionResult> DisconnectSpotify([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = RouteConstants.SpotifyDisconnect)] HttpRequestData req, string userId)
        {
            Console.WriteLine("Disconnecting Spotify for Supabase user: " + userId);
            
            var disconnectResponse = await _spotifyService.DisconnectSpotify(userId);
            if (disconnectResponse.Status != HttpStatusCode.OK)
            {
                return NotFound($"Failed to disconnect Spotify: {disconnectResponse.ErrorMessage}");
            }
            
            return Ok(new { message = "Spotify disconnected successfully" });
        }
    }
}