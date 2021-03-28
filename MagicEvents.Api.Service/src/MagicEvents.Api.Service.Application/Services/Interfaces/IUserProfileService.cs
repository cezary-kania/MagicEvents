using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileBaseDto> GetProfileAsync(Guid userId);
        Task<byte[]> GetProfileImageAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto profileDto);
        Task UpdateProfileImageAsync(Guid userId, byte[] imageData);
    }
}