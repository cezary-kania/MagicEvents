using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;
using Moq;
using Xunit;

namespace MagicEvents.Api.Service.UnitTests.Application.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IMapper> _mapperStub = new();
        private readonly Mock<IUserRepository> _userRepositoryStub = new();
        
        [Fact]
        public void GetAsync_WhenUserIdNotValid_ShouldRaiseServiceException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User) null);
            var userService = new UserService(_mapperStub.Object, _userRepositoryStub.Object);
            // Act
            Func<Task> getAsyncFunc = async () => await userService.GetAsync(userId);
            // Assert
            getAsyncFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.UserNotFound);
        }

        [Fact]
        public async Task GetAsync_WhenUserIdIsValid_ShouldReturnUserDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User(userId, null, null);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingUser);
            _mapperStub.Setup(x => x.Map<UserDto>(existingUser))
                .Returns(new UserDto
                {
                    Id = existingUser.Id,
                    Profile = new UserProfileDto
                    {
                        FirstName = existingUser.Profile?.FirstName,
                        LastName = existingUser.Profile?.LastName,
                        Informations = existingUser.Profile?.Informations,
                    },
                    Identity = new UserIdentityDto
                    {
                        Email = existingUser.Identity?.Email
                    }
                });
            var userService = new UserService(_mapperStub.Object, _userRepositoryStub.Object);
            // Act
            var result = await userService.GetAsync(userId);
            // Assert
            result.Should()
                .BeEquivalentTo(existingUser,
                options => options.ExcludingMissingMembers());
        }
    }
}