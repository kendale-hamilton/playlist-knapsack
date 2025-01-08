using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;
        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }
        [HttpGet("users/{userId}/playlists/{playlistId}/tracks")]
        public async Task<ActionResult> GetPlaylistTracks(string userId, string playlistId)
        {
            var playlistTracks = await _playlistService.GetPlaylistTracks(playlistId);
            return Ok(playlistTracks);
        }
    }
}