using System;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.Domain.ValueObjects;

namespace MagicEvents.Api.Service.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IImageProcessor _imageProcessor;
        public UserProfileService(IMapper mapper, IUserRepository userRepository, IImageProcessor imageProcessor)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _imageProcessor = imageProcessor;
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
            if(!_imageProcessor.IsValidImage(imageData))
            {
                throw new ServiceException(ExceptionMessage.File.InvalidInputFile);
            }
            imageData = await _imageProcessor.CreateThumbnailAsync(imageData);
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