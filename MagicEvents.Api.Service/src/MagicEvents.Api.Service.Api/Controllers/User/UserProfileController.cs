using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Api.Common;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.Api.Service.Api.Controllers.User
{
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : SecuredControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("{userId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserProfileBaseDto), 200)]
        [ProducesResponseType(typeof(object),404)]
        public async Task<IActionResult> GetProfile([FromRoute]Guid userId)
        {
            var userProfile = await _userProfileService.GetProfileAsync(userId);
            if(userProfile is null) return NotFound();
            return Ok(userProfile);
        }

        [HttpGet("{userId}/profileImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object),404)]
        public async Task<IActionResult> GetProfileImage([FromRoute]Guid userId)
        {
            var profileImage = await _userProfileService.GetProfileImageAsync(userId);
            if(profileImage is null) return NotFound();
            return File(profileImage,"image/png",$"user-{userId}.jpg");
        }
        
        [Authorize]
        [HttpPatch("profileImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> UpdateProfileImage([FromForm] IFormFile file)
        {
            if(file is null) 
            {
                return BadRequest(new { error = "File can't be null" });
            }
            if(file.IsLargeFile())
            {
                return BadRequest(new {error = "File size limit exceeded"});    
            }
            if(!file.ContainImage())
            {
                return BadRequest(new {error = "File is not image"});
            }
            var binaryData = await file.ToByteArray();
            await _userProfileService.UpdateProfileImageAsync(UserId, binaryData);
            return NoContent();
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> UpdateProfile([FromBody]UpdateProfileDto profileDto)
        {
            await _userProfileService.UpdateProfileAsync(UserId, profileDto);
            return NoContent();
        }
    }
}