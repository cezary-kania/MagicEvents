using System;

namespace MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent
{
    public class CreateEventDto
    {
        public string Title { get; set; }
        public string Description { get; set;}
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
    }
}