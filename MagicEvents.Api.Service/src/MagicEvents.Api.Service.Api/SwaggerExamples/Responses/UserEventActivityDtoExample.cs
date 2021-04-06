using System;
using MagicEvents.Api.Service.Application.DTOs.Users;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Responses
{
    public class UserEventActivityDtoExample : IExamplesProvider<UserEventActivityDto>
    {
        public UserEventActivityDto GetExamples()
        {
            return new UserEventActivityDto
            {
                EventId = Guid.NewGuid(),
                Role = "StandardParticipant"
            };
        }
    }
}