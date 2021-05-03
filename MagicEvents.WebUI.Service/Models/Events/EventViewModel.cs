using System;

namespace MagicEvents.WebUI.Service.Models.Events
{
    public class EventViewModel
    {
        public Guid Id { get; set; }
        public string Organizer { get; set; }
        public string Title { get; set; }
        public string Description { get; set;}
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string Status { get; set; }
    }
}