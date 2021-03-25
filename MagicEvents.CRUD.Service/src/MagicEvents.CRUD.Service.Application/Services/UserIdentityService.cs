using System;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity.LoginUser;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity.RegisterUser;
using MagicEvents.CRUD.Service.Application.Exceptions;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using MagicEvents.CRUD.Service.Domain.Entities;
using MagicEvents.CRUD.Service.Domain.Repositories;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Application.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IEncryptService _encryptService;
        private readonly IJwtService _jwtService;
        public UserIdentityService(IMapper mapper, IUserRepository userRepository, IJwtService jwtService, IEncryptService encryptService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _encryptService = encryptService;
        }
        public async Task<AuthTokenDto> Login(LoginUserDto loginUserDto)
        {
            var user = await _userRepository.GetAsync(loginUserDto.Email);
            if(user is null || !_encryptService.ValidatePassword(user, loginUserDto.Password))
            {
                throw new ServiceException(ExceptionMessage.User.InvalidCredentials);
            }
            return _jwtService.GenerateToken(user);
        }

        public async Task<AuthTokenDto> Register(RegisterUserDto registerUserDto)
        {
            var user = await _userRepository.GetAsync(registerUserDto.Email);
            if(user is not null)
            {
                throw new ServiceException(ExceptionMessage.User.EmailAlreadyUsed);
            }
            var userId = Guid.NewGuid();
            var userSalt = _encryptService.GenerateSalt();
            var passwordHash = _encryptService.GetHash(registerUserDto.Password, userSalt);
            user = new User(userId, 
                new UserIdentity 
                {
                    Email = registerUserDto.Email,
                    Salt = userSalt,
                    Password = passwordHash
                });
            await _userRepository.AddAsync(user);
            return _jwtService.GenerateToken(user);
        }
    }
}