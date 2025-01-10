using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IServiceBase _service;
        public PlaylistsController(IServiceBase serviceBase)
        {
            _service = serviceBase;
        }
        [HttpGet("users/{userId}/playlists/{playlistId}/tracks")]
        public async Task<ActionResult> GetPlaylistTracks(string userId, string playlistId)
        {
            Console.WriteLine("Getting Playlist Tracks...");
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            Console.WriteLine($"token: {accessToken}");
            var playlistTracks = await _service.FetchPlaylistTracks(playlistId, accessToken);
            Console.WriteLine($"PlaylistTracks: {playlistTracks.Items[0].Track.DurationMs}");
            return Ok(playlistTracks);
        }
    }
}