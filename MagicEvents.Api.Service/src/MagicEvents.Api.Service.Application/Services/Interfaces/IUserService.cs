using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetAsync(Guid userId);
    }
}