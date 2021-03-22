using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity.LoginUser;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity.RegisterUser;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IUserIdentityService
    {
        Task<AuthTokenDto> Register(RegisterUserDto registerUserDto);
        Task<AuthTokenDto> Login(LoginUserDto loginUserDto);

        // TODO: Add Token Refreshing 
        // TODO: Add Facebook Auth
    }
}