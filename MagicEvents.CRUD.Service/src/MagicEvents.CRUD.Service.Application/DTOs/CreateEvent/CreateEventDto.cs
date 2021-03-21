using System;

namespace MagicEvents.CRUD.Service.Application.DTOs.CreateEvent
{
    public class CreateEventDto
    {
        public string Title { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
    }
}