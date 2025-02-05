
using Microsoft.AspNetCore.Mvc;
using Models.Knapsack;
using Services;

namespace Controllers
{
    [ApiController]
    [Route("api/spotify")]
    public class SpotifyController : ControllerBase
    {
        private readonly IServiceBase _service;
        public SpotifyController(IServiceBase serviceBase)
        {
            _service = serviceBase;
        }
        [HttpGet("users/{userId}/playlists")]
        public async Task<ActionResult> GetUserPlaylists(string userId)
        {
            Console.WriteLine("Getting User Playlists...");
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            List<PlaylistDetails> userPlaylists = await _service.GetUserPlaylists(userId, accessToken);
            return Ok(userPlaylists);
        }
        [HttpGet("playlists/{playlistId}")]
        public async Task<ActionResult> GetPlaylist(string userId, string playlistId)
        {
            Console.WriteLine("Getting Playlist Tracks...");
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            PlaylistDetails details = await _service.GetPlaylistDetails(playlistId, accessToken);
            List<Track> playlistTracks = await _service.GetPlaylistTracks(playlistId, accessToken);
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