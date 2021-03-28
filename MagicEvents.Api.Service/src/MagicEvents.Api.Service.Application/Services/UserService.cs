using System;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Repositories;

namespace MagicEvents.Api.Service.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public UserService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }
        public async Task<UserDto> GetAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
            return _mapper.Map<UserDto>(user);
        }
    }
}