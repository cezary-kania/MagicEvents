using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.Api.Service.Api.Controllers.User
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : SecuredControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("userData")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _userService.GetAsync(UserId);
            if(user is null) return NotFound();
            return Ok(user);
        }
    }
}