using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        public async Task<IActionResult> GetActivities()
            => Ok(await _userActivityService.GetActivitiesAsync(UserId));

        [HttpPost("{eventId}")]
        public async Task<IActionResult> RegisterOnEvent([FromRoute] Guid eventId)
        {
            await _userActivityService.RegisterOnEventAsync(UserId, eventId);
            return NoContent();
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> LeaveEvent([FromRoute] Guid eventId)
        {
            await _userActivityService.LeaveEventAsync(UserId, eventId);
            return NoContent();
        }
    }
}