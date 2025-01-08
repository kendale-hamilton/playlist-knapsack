using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;
        public UsersController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }
        [HttpGet("users/{userId}")]
        public async Task<ActionResult> GetPlaylists(string userId)
        {
            var playlists = await _playlistService.GetPlaylists(userId);
            return Ok(playlists);
        }
    }
}