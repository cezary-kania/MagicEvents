using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Api.Common;
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
        public async Task<IActionResult> GetProfile([FromRoute]Guid userId)
        {
            var userProfile = await _userProfileService.GetProfileAsync(userId);
            if(userProfile is null) return NotFound();
            return Ok(userProfile);
        }

        [HttpGet("{userId}/profileImage")]
        public async Task<IActionResult> GetProfileImage([FromRoute]Guid userId)
        {
            var profileImage = await _userProfileService.GetProfileImageAsync(userId);
            if(profileImage is null) return NotFound();
            return File(profileImage,"image/jpeg",$"user-{userId}.jpg");
        }
        
        [Authorize]
        [HttpPatch("profileImage")]
        public async Task<IActionResult> UpdateProfileImage([FromForm] IFormFile file)
        {
            //TODO: Add image validation and normalization (required content-type: image/jpeg)
            var binaryData = await FileConverter.ConvertToByteArray(file);
            await _userProfileService.UpdateProfileImageAsync(UserId, binaryData);
            return NoContent();
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody]UpdateProfileDto profileDto)
        {
            await _userProfileService.UpdateProfileAsync(UserId, profileDto);
            return NoContent();
        }
    }
}