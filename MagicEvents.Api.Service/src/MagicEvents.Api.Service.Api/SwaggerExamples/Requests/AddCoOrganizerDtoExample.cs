using System;
using MagicEvents.Api.Service.Application.DTOs.Events.AddCoOrganizer;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Requests
{
    public class AddCoOrganizerDtoExample : IExamplesProvider<AddCoOrganizerDto>
    {
        public AddCoOrganizerDto GetExamples()
        {
            return new AddCoOrganizerDto 
            {
                UserId = Guid.NewGuid()
            };
        }
    }
}