using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.LoginUser;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.RegisterUser;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.Api.Service.Api.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IUserIdentityService _userIdentityService;
        public IdentityController(IUserIdentityService userIdentityService)
        {
            _userIdentityService = userIdentityService;
        }

        [HttpPost("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AuthTokenDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RegisterAsync(RegisterUserDto registerUserDto)
            => Ok(await _userIdentityService.RegisterAsync(registerUserDto));

        [HttpPost("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AuthTokenDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> LoginAsync(LoginUserDto loginUserDto)
            => Ok(await _userIdentityService.LoginAsync(loginUserDto));
    }
}