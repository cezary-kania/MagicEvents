using MagicEvents.Api.Service.Application.DTOs.Users.Identity.RegisterUser;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Requests
{
    public class RegisterUserDtoExample : IExamplesProvider<RegisterUserDto>
    {
        public RegisterUserDto GetExamples()
        {
            return new RegisterUserDto
            {
                Email = "example@example.com",
                Password = "P4$$w0rd1234"
            };
        }
    }
}