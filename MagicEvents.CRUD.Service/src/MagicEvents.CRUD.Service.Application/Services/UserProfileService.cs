using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users.UpdateProfile;
using MagicEvents.CRUD.Service.Application.Exceptions;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using MagicEvents.CRUD.Service.Domain.Repositories;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        public UserProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<byte[]> GetProfileImage(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            CheckIfUserExists(user);
            return user.Profile.Image.BinaryData;
        }

        public async Task UpdatePhoto(Guid userId, byte[] imageData)
        {
            var user = await _userRepository.GetAsync(userId);
            CheckIfUserExists(user);
            user.Profile.Image.BinaryData = imageData;
            await _userRepository.UpdateAsync(user); 
        }

        public async Task UpdateProfile(Guid userId, UpdateProfileDto profileDto)
        {
            var user = await _userRepository.GetAsync(userId);
            CheckIfUserExists(user);
            if(user.Profile is null) 
            {
                user.Profile = new UserProfile();
            }
            user.Profile.FirstName = profileDto.FirstName;
            user.Profile.LastName = profileDto.LastName;
            user.Profile.Informations = profileDto.Informations;
            await _userRepository.UpdateAsync(user);
        }

        private static void CheckIfUserExists(Domain.Entities.User user)
        {
            if (user is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
        }
    }
}