using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase
    {
        private readonly IServiceBase _service;
        public UsersController(IServiceBase serviceBase)
        {
            _service = serviceBase;
        }
        [HttpGet("users/{userId}")]
        public async Task<ActionResult> GetPlaylists(string userId)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var playlists = await _service.FetchPlaylists(userId, accessToken);
            return Ok(playlists);
        }
    }
}