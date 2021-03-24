using System;
using System.Collections.Generic;

namespace MagicEvents.CRUD.Service.Application.DTOs.Events
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public Guid OrganizerId { get; set; }
        public EventParticipantsDto Participants { get; set; }
        public string Title { get; set; }
        public string Description { get; set;}
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string Status { get; set; }
    }
}