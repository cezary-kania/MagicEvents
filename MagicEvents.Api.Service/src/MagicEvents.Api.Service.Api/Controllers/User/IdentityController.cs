using System.Threading.Tasks;
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
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
            => Ok(await _userIdentityService.RegisterAsync(registerUserDto));

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
            => Ok(await _userIdentityService.LoginAsync(loginUserDto));
    }
}