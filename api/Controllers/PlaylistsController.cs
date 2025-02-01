using API.Services;
using Microsoft.AspNetCore.Mvc;
using Models;

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
            SimpPlaylist playlist = await _service.FetchPlaylist(playlistId, accessToken);
            List<SimpTrack> playlistTracks = await _service.FetchPlaylistTracks(playlistId, accessToken);
            int duration = 0;
            foreach (SimpTrack t in playlistTracks)
            {
                duration += t.DurationMs ?? 0;
            }
            playlist.Details.DurationMs = duration;
            SimpPlaylist playlistInfo= new SimpPlaylist
            {
                Details = playlist.Details,
                Tracks = playlistTracks
            };
            Console.WriteLine($"Playlist info: {playlistInfo}");
            return Ok(playlistInfo);
        }
    }
}