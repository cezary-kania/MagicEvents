using System;

namespace MagicEvents.CRUD.Service.Application.DTOs.UpdateEvent
{
    public class UpdateEventDto
    {
        public string Title { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
    }
}