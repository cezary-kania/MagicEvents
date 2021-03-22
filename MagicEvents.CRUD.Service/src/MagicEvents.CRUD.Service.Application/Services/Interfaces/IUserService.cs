using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users;
using MagicEvents.CRUD.Service.Application.DTOs.Users.UpdateProfile;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> Get(Guid userId);
    }
}