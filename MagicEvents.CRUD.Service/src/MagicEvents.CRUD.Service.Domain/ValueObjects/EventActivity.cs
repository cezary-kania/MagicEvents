using System;

namespace MagicEvents.CRUD.Service.Domain.ValueObjects
{
    public class EventActivity
    {
        public Guid EventId { get; set; }
        public string Role { get; set; }
    }
}