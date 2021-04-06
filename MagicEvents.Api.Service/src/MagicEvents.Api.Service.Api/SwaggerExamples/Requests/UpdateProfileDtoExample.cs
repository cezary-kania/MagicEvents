using MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Requests
{
    public class UpdateProfileDtoExample : IExamplesProvider<UpdateProfileDto>
    {
        public UpdateProfileDto GetExamples()
        {
            return new UpdateProfileDto
            {
                FirstName = "firstname",
                LastName = "lastname",
                Informations = "User informations"
            };
        }
    }
}