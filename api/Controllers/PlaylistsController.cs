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
            var playlistTracks = await _service.FetchPlaylistTracks(playlistId, accessToken);
            return Ok(playlistTracks);
        }
    }
}