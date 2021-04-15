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
        private readonly IImageProcessor _imageProcessor;
        public UserProfileController(IUserProfileService userProfileService, IImageProcessor imageProcessor)
        {
            _userProfileService = userProfileService;
            _imageProcessor = imageProcessor;
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
            if(!_imageProcessor.IsFileSizeValid(file.Length))
            {
                return BadRequest(new {error = "File size limit exceeded"});    
            }
            var binaryData = await FileConverter.ConvertToByteArray(file);
            if(!_imageProcessor.IsValidImage(binaryData, file.ContentType))
            {
                return BadRequest(new {error = "Invalid input file"});
            }
            var thumbnailData = await _imageProcessor.CreateThumbnail(binaryData);
            await _userProfileService.UpdateProfileImageAsync(UserId, thumbnailData);
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