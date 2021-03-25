using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity.LoginUser;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity.RegisterUser;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.CRUD.Service.Api.Controllers.User
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
            => Ok(await _userIdentityService.Register(registerUserDto));

        [HttpPost("login")]
        public async Task<IActionResult> Register(LoginUserDto loginUserDto)
            => Ok(await _userIdentityService.Login(loginUserDto));
    }
}