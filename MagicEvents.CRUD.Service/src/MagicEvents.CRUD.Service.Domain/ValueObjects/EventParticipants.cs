using System;
using System.Collections.Generic;

namespace MagicEvents.CRUD.Service.Domain.ValueObjects
{
    public class EventParticipants
    {
        public List<Guid> StandardParticipants { get; set; } = new List<Guid>();
        public List<Guid> CoOrganizers { get; set; } = new List<Guid>();
    }
}