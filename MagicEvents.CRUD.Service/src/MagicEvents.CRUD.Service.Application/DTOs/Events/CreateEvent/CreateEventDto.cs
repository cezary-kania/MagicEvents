using System;

namespace MagicEvents.CRUD.Service.Application.DTOs.Events.CreateEvent
{
    public class CreateEventDto
    {
        public Guid OrganizerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set;}
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
    }
}