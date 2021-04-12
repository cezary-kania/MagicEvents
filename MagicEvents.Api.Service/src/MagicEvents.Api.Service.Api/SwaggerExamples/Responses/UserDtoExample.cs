using System;
using System.Collections.Generic;
using MagicEvents.Api.Service.Application.DTOs.Users;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Responses
{
    public class UserDtoExample : IExamplesProvider<UserDto>
    {
        public UserDto GetExamples()
        {
            return new UserDto 
            {
                Id = Guid.NewGuid(),
                Identity = new UserIdentityDto
                {
                    Email = "example@example.com"
                },
                Profile = new UserProfileDto
                {
                    FirstName = "firstname",
                    LastName = "lastname",
                    Informations = "User informations"
                }
            };
        }
    }
}