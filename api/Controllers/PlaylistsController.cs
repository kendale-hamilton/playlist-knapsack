using Services;
using Microsoft.AspNetCore.Mvc;
using Models.Knapsack;

namespace API.Controllers
{
    [ApiController]
    [Route("api/spotify")]
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