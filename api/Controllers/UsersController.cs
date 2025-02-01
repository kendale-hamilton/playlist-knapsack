using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/spotify")]
    public class UsersController : ControllerBase
    {
        private readonly IServiceBase _service;
        public UsersController(IServiceBase serviceBase)
        {
            _service = serviceBase;
        }

        [HttpGet("users/{userId}/playlists")]
        public async Task<ActionResult> GetPlaylists(string userId)
        {
            Console.WriteLine("Getting User Playlists...");
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userPlaylists = await _service.GetUserPlaylists(userId, accessToken);
            return Ok(userPlaylists);
        }
    }
}