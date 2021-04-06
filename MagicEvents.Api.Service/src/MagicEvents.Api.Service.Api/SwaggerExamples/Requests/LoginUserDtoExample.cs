using MagicEvents.Api.Service.Application.DTOs.Users.Identity.LoginUser;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Requests
{
    public class LoginUserDtoExample : IExamplesProvider<LoginUserDto>
    {
        public LoginUserDto GetExamples()
        {
            return new LoginUserDto
            {
                Email = "example@example.com",
                Password = "P4$$w0rd1234"
            };
        }
    }
}