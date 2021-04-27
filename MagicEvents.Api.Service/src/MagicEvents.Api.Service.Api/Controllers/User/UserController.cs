using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(object),404)]
        public async Task<IActionResult> GetUserAsync()
        {
            var user = await _userService.GetAsync(UserId);
            if(user is null) return NotFound();
            return Ok(user);
        }

        [HttpGet("userData/{userId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(object),404)]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid userId)
        {
            var user = await _userService.GetAsync(userId);
            if(user is null) return NotFound();
            return Ok(user);
        }
    }
}