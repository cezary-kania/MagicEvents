using System;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Users;
using MagicEvents.CRUD.Service.Application.DTOs.Users.UpdateProfile;
using MagicEvents.CRUD.Service.Application.Exceptions;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using MagicEvents.CRUD.Service.Domain.Entities;
using MagicEvents.CRUD.Service.Domain.Repositories;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public UserProfileService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<UserProfileBaseDto> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            CheckIfUserExists(user);
            return _mapper.Map<UserProfileBaseDto>(user.Profile);
        }
        public async Task<byte[]> GetProfileImageAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            CheckIfUserExists(user);
            return user.Profile.Image?.BinaryData;
        }

        public async Task UpdateProfileImageAsync(Guid userId, byte[] imageData)
        {
            var user = await _userRepository.GetAsync(userId);
            CheckIfUserExists(user);
            user.Profile.Image = new UserProfileImage 
            { 
                UserId = userId, 
                BinaryData = imageData
            }; 
            await _userRepository.UpdateAsync(user); 
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto profileDto)
        {
            var user = await _userRepository.GetAsync(userId);
            CheckIfUserExists(user);
            user.Profile.FirstName = profileDto.FirstName;
            user.Profile.LastName = profileDto.LastName;
            user.Profile.Informations = profileDto.Informations;
            await _userRepository.UpdateAsync(user);
        }

        private static void CheckIfUserExists(User user)
        {
            if (user is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
        }
    }
}