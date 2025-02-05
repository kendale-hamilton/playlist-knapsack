
using Microsoft.AspNetCore.Mvc;
using Models.Knapsack;
using Services.SpotifyService;

namespace Controllers.SpotifyController
{
    [ApiController]
    [Route("api/spotify")]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;
        public SpotifyController(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }
        [HttpGet("users/{userId}/playlists")]
        public async Task<ActionResult> GetUserPlaylists(string userId)
        {
            Console.WriteLine("Getting User Playlists...");
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            List<PlaylistDetails> userPlaylists = await _spotifyService.GetUserPlaylists(userId, accessToken);
            return Ok(userPlaylists);
        }
        [HttpGet("playlists/{playlistId}")]
        public async Task<ActionResult> GetPlaylist(string playlistId)
        {
            Console.WriteLine("Getting Playlist Tracks...");
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            PlaylistDetails details = await _spotifyService.GetPlaylistDetails(playlistId, accessToken);
            List<Track> playlistTracks = await _spotifyService.GetPlaylistTracks(playlistId, accessToken);
            int duration = 0;
            foreach (Track t in playlistTracks)
            {
                duration += t.DurationMs ?? 0;
            }
            details.DurationMs = duration;
            Playlist playlistInfo= new Playlist
            {
                Details = details,
                Tracks = playlistTracks
            };
            return Ok(playlistInfo);
        }
    }
}