using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.LoginUser;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.RegisterUser;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IUserIdentityService
    {
        Task<AuthTokenDto> RegisterAsync(RegisterUserDto registerUserDto);
        Task<AuthTokenDto> LoginAsync(LoginUserDto loginUserDto);
    }
}