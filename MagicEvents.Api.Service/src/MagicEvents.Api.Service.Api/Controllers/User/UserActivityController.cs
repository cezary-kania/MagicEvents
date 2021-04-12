using System;
using System.Collections.Generic;
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
    public class UserActivityController : SecuredControllerBase
    {
        private readonly IUserActivityService _userActivityService;
        public UserActivityController(IUserActivityService userActivityService)
        {
            _userActivityService = userActivityService;
        }

        [HttpGet("{userId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<UserEventActivityDto>), 200)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> GetActivities([FromRoute]Guid userId)
            => Ok(await _userActivityService.GetActivitiesAsync(userId));

        [HttpPost("{eventId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> RegisterOnEvent([FromRoute] Guid eventId)
        {
            await _userActivityService.RegisterOnEventAsync(UserId, eventId);
            return NoContent();
        }

        [HttpDelete("{eventId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> LeaveEvent([FromRoute] Guid eventId)
        {
            await _userActivityService.LeaveEventAsync(UserId, eventId);
            return NoContent();
        }
    }
}