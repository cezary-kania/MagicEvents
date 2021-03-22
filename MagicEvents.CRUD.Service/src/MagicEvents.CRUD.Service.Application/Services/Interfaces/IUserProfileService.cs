using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users.UpdateProfile;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task UpdateProfile(Guid userId, UpdateProfileDto profileDto);
        Task UpdatePhoto(Guid userId, byte[] imageData);
    }
}