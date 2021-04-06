using MagicEvents.Api.Service.Application.DTOs.Users;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Responses
{
    public class UserProfileBaseDtoExample : IExamplesProvider<UserProfileBaseDto>
    {
        public UserProfileBaseDto GetExamples()
        {
            return new UserProfileBaseDto
            {
                FirstName = "firstname",
                LastName = "lastname"
            };
        }
    }
}