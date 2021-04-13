using System;
using System.Collections.Generic;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginatedResponse;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api.SwaggerExamples.Responses
{
    public class PaginatedEventsResponseDtoExample : IExamplesProvider<PaginatedResponseDto<EventDto>>
    {
        public PaginatedResponseDto<EventDto> GetExamples()
        {
            var items = new EventDto[] 
            {
                GetEventExample(),
                GetEventExample()
            };
            return new PaginatedResponseDto<EventDto>
            {
                Items = items,
                TotalCount = 2,
                PageIndex = 0,
                TotalPages = 1,
                HasNext = false,
                HasPrevious = false
            };
        }

        private EventDto GetEventExample()
        {
            return new EventDto 
            {
                Id = Guid.NewGuid(),
                OrganizerId = Guid.NewGuid(),
                Participants = new EventParticipantsDto
                {
                    StandardParticipants = new List<Guid>(),
                    CoOrganizers = new List<Guid>(),
                },
                Title = "mollit dolore cupidatat ",
                Description = "in sit",
                StartsAt = DateTime.Now,
                EndsAt = DateTime.Now,
                Status = "Open"
            };
        }
    }
}