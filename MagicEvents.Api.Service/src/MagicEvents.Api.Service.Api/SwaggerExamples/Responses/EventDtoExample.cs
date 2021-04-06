using System;
using System.Collections.Generic;
using MagicEvents.Api.Service.Application.DTOs.Events;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Responses
{
    public class EventDtoExample : IExamplesProvider<EventDto>
    {
        public EventDto GetExamples()
        {
            return new EventDto 
            {
                Id = Guid.Parse("3b7627ab-91c3-4df0-a5df-a37189e3088d"),
                OrganizerId = Guid.Parse("8050f864-1f61-48ca-8043-1fb176bc908f"),
                Participants = new EventParticipantsDto
                {
                    StandardParticipants = new List<Guid>(),
                    CoOrganizers = new List<Guid>(),
                },
                Title = "mollit dolore cupidatat ",
                Description = "in sit",
                StartsAt = DateTime.Parse("2021-06-13T09:35:38.817Z"),
                EndsAt = DateTime.Parse("2021-09-17T04:05:10.997Z"),
                Status = "Open"
            };
        }
    }
}