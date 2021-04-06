using System;
using MagicEvents.Api.Service.Application.DTOs.Events.UpdateEvent;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Requests
{
    public class UpdateEventDtoExample : IExamplesProvider<UpdateEventDto>
    {
        public UpdateEventDto GetExamples()
        {
            return new UpdateEventDto
            {
                Title = "Example title",
                Description = "Example description",
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(3),
            };
        }
    }
}