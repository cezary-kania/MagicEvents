using System;
using System.Collections.Generic;

namespace MagicEvents.WebUI.Service.DTOs.Events
{
    public class EventParticipantsDto
    {
        public IEnumerable<Guid> StandardParticipants { get; set; }
        public IEnumerable<Guid> CoOrganizers { get; set; }
    }
}