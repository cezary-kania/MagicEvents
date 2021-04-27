using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.Domain.ValueObjects;
using Moq;
using Xunit;

namespace MagicEvents.Api.Service.UnitTests.Application.Services
{
    public class UserProfileServiceTests
    {
        private readonly Mock<IMapper> _mapperStub = new();
        private readonly Mock<IUserRepository> _userRepositoryStub = new();
        private readonly Mock<IImageProcessor> _imageProcessorStub = new();

        [Fact]
        public void GetProfileAsync_WhenUserNotExist_ShouldRaiseServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task<UserProfileBaseDto>> getProfileFunc = () => userProfileService.GetProfileAsync(userId);
            // Assert
            getProfileFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
                
        }

        [Fact]
        public async Task GetProfileAsync_WhenUserExist_ShouldReturnProfileInfo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, new UserProfile 
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString()
            });
            var userProfileDto = new UserProfileBaseDto 
            {
                FirstName = existingUser.Profile.FirstName,
                LastName = existingUser.Profile.LastName
            };
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _mapperStub.Setup(x => x.Map<UserProfileBaseDto>(existingUser.Profile))
                .Returns(userProfileDto);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            var result = await userProfileService.GetProfileAsync(userId);
            // Assert
            result.Should()
                .BeEquivalentTo(userProfileDto);                
        }

        [Fact]
        public async Task GetProfileImageAsync_WhenImageNotExist_ShouldReturnNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            var result = await userProfileService.GetProfileImageAsync(userId);
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetProfileImageAsync_WhenUserSetImage_ShouldReturnImageData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var imageData = Guid.NewGuid().ToByteArray();
            var existingUser = new User(userId, null, new UserProfile 
            {
                Image = new UserProfileImage
                {
                    UserId = userId,
                    BinaryData = imageData
                }
            });
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            var result = await userProfileService.GetProfileImageAsync(userId);
            // Assert
            result.Should()
                .BeEquivalentTo(imageData);                
        }

        [Fact]
        public void UpdateProfileImageAsync_WhenUserNotExist_ShouldRaiseServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var imageData = Guid.NewGuid().ToByteArray();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> getProfileFunc = () => 
                userProfileService.UpdateProfileImageAsync(userId, imageData);
            // Assert
            getProfileFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public void UpdateProfileImageAsync_WhenBinaryDataIsNotValid_ShouldRaiseServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null);
            var imageData = Guid.NewGuid().ToByteArray();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _imageProcessorStub.Setup(x => x.IsValidImage(It.IsAny<byte[]>()))
                .Returns(false);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> updateProfileImageFunc = () => 
                userProfileService.UpdateProfileImageAsync(userId, imageData);
            // Assert
            updateProfileImageFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.File.InvalidInputFile);
        }
        
        [Fact]
        public async Task UpdateProfileImageAsync_WhenBinaryDataValid_ShouldCreateThumbnail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null);
            var imageData = Guid.NewGuid().ToByteArray();
            var thumbnailData = Guid.NewGuid().ToByteArray();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _imageProcessorStub.Setup(x => x.IsValidImage(It.IsAny<byte[]>()))
                .Returns(true);
            _imageProcessorStub.Setup(x => x.CreateThumbnailAsync(It.IsAny<byte[]>()))
                .ReturnsAsync(thumbnailData);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            await userProfileService.UpdateProfileImageAsync(userId, imageData);
            // Assert
            existingUser.Profile.Image.BinaryData
                .Should().BeEquivalentTo(thumbnailData);
        }

        [Fact]
        public void UpdateProfileAsync_WhenUserNotExist_ShouldRaiseServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateProfileDto = new UpdateProfileDto
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Informations = Guid.NewGuid().ToString(),
            };
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            Func<Task> getProfileFunc = () => 
                userProfileService.UpdateProfileAsync(userId, updateProfileDto);
            // Assert
            getProfileFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public async Task UpdateProfileAsync_WhenUserExists_ShouldUpdateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateProfileDto = new UpdateProfileDto
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Informations = Guid.NewGuid().ToString(),
            };
            var existingUser = new User(userId);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            var userProfileService = new UserProfileService(
                _mapperStub.Object,
                _userRepositoryStub.Object,
                _imageProcessorStub.Object);
            // Act
            await userProfileService.UpdateProfileAsync(userId, updateProfileDto);
            // Assert
            existingUser.Profile.Should()
                .BeEquivalentTo(updateProfileDto,
                    options => options.ExcludingMissingMembers());
        }
    }
}