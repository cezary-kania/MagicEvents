using System;

namespace MagicEvents.WebUI.Service.DTOs.Events
{
    public class UpdateEventDto
    {
        public string Title { get; set; }
        public string Description { get; set;}
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
    }
}