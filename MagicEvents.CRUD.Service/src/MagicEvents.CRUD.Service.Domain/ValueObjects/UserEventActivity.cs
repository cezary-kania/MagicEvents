using System;

namespace MagicEvents.CRUD.Service.Domain.ValueObjects
{
    public class UserEventActivity
    {
        public Guid EventId { get; set; }
        public string Role { get; set; }
    }
}