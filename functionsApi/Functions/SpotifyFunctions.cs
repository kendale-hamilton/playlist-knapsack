
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
    }
}