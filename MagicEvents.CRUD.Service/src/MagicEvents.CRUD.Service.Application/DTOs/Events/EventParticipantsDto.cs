using System;
using System.Collections.Generic;

namespace MagicEvents.CRUD.Service.Application.DTOs.Events
{
    public class EventParticipantsDto
    {
        public IEnumerable<Guid> StandardParticipants { get; set; }
        public IEnumerable<Guid> CoOrganizers { get; set; }
    }
}