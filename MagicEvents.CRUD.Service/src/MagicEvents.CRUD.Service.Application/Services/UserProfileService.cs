using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users.UpdateProfile;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;

namespace MagicEvents.CRUD.Service.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        public Task<byte[]> GetProfileImage(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePhoto(Guid userId, byte[] imageData)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProfile(Guid userId, UpdateProfileDto profileDto)
        {
            throw new NotImplementedException();
        }
    }
}