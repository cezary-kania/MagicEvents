using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users;
using MagicEvents.CRUD.Service.Application.DTOs.Users.UpdateProfile;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileBaseDto> GetProfileAsync(Guid userId);
        Task<byte[]> GetProfileImageAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto profileDto);
        Task UpdateProfileImageAsync(Guid userId, byte[] imageData);
    }
}