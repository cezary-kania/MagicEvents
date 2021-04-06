using System;
using MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Requests
{
    public class CreateEventDtoExample : IExamplesProvider<CreateEventDto>
    {
        public CreateEventDto GetExamples()
        {
            return new CreateEventDto 
            {
                Title = "Example title",
                Description = "Example description",
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(3),
            };
        }
    }
}