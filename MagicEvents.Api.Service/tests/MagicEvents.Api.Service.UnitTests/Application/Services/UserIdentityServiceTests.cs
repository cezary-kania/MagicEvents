using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.LoginUser;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.RegisterUser;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;
using Moq;
using Xunit;

namespace MagicEvents.Api.Service.UnitTests.Application.Services
{
    public class UserIdentityServiceTests
    {
        private readonly Mock<IMapper> _mapperStub = new();
        private readonly Mock<IUserRepository> _userRepositoryStub = new();
        private readonly Mock<IEncryptService> _encryptServiceStub = new();
        private readonly Mock<IJwtService> _jwtServiceStub = new();
        

        [Fact]
        public void LoginAsync_WhenUserNotExist_ShouldThrowServiceException()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "test@email.com",
                Password = Guid.NewGuid().ToString()
            };
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((User) null);
            var identityService = new UserIdentityService(
                _mapperStub.Object,
                _userRepositoryStub.Object, 
                _jwtServiceStub.Object,
                _encryptServiceStub.Object);
            // Act
            Func<Task> loginFunc = () => identityService.LoginAsync(loginDto);
            // Assert
            loginFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.InvalidCredentials);
        }

        [Fact]
        public void LoginAsync_WhenUserCredentialsAreNotValid_ShouldThrowServiceException()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "test@email.com",
                Password = Guid.NewGuid().ToString()
            };
            var existingUser = new User(Guid.NewGuid(), null, null);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);
            _encryptServiceStub.Setup(x => x.ValidatePassword(existingUser, loginDto.Password))
                .Returns(false);
            var identityService = new UserIdentityService(
                _mapperStub.Object,
                _userRepositoryStub.Object, 
                _jwtServiceStub.Object,
                _encryptServiceStub.Object);
            // Act
            Func<Task> loginFunc = () => identityService.LoginAsync(loginDto);
            // Assert
            loginFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.InvalidCredentials);
        }

        [Fact]
        public async Task LoginAsync_WhenUserCredentialsAreValid_ShouldReturnJwt()
        {
            // Arrange
            var loginDto = new LoginUserDto
            {
                Email = "test@email.com",
                Password = Guid.NewGuid().ToString()
            };
            var existingUser = new User(Guid.NewGuid(), null, null);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);
            _encryptServiceStub.Setup(x => x.ValidatePassword(existingUser, loginDto.Password))
                .Returns(true);
            var authDto = new AuthTokenDto
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
                Expiry = DateTime.UtcNow.AddMinutes(15)
            };
            _jwtServiceStub.Setup(x => x.GenerateToken(existingUser))
                .Returns(authDto);
            var identityService = new UserIdentityService(
                _mapperStub.Object,
                _userRepositoryStub.Object, 
                _jwtServiceStub.Object,
                _encryptServiceStub.Object);
            // Act
            var result = await identityService.LoginAsync(loginDto);
            // Assert
            result.Should()
                .BeEquivalentTo(authDto);
        }

        [Fact]
        public void RegisterAsync_WhenUserEmailReserved_ShouldThrowServiceException()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Email = "test@email.com",
                Password = Guid.NewGuid().ToString()
            };
            var existingUser = new User(Guid.NewGuid(), null, null);
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);
            var identityService = new UserIdentityService(
                _mapperStub.Object,
                _userRepositoryStub.Object, 
                _jwtServiceStub.Object,
                _encryptServiceStub.Object);
            // Act
            Func<Task> registerFunc = () => identityService.RegisterAsync(registerDto);
            // Assert
            registerFunc.Should()
                .Throw<ServiceException>()
                .WithMessage(ExceptionMessage.User.EmailAlreadyUsed);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailNotReserved_ShouldRegisterUser()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Email = "test@email.com",
                Password = Guid.NewGuid().ToString()
            };
            _userRepositoryStub.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((User) null);
            _encryptServiceStub.Setup(x => x.GenerateSalt())
                .Returns(Guid.NewGuid().ToString());
            _encryptServiceStub.Setup(x => x.GetHash(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Guid.NewGuid().ToString());
            var authDto = new AuthTokenDto
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
                Expiry = DateTime.UtcNow.AddMinutes(15)
            };
            _jwtServiceStub.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns(authDto);
            var identityService = new UserIdentityService(
                _mapperStub.Object,
                _userRepositoryStub.Object, 
                _jwtServiceStub.Object,
                _encryptServiceStub.Object);
            // Act
            var result = await identityService.RegisterAsync(registerDto);
            // Assert
            _userRepositoryStub.Verify(x => x.AddAsync(It.IsAny<User>()));
            result.Should()
                .BeEquivalentTo(authDto);

        }
    }
}