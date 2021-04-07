using System;

namespace MagicEvents.Api.Service.Domain.ValueObjects
{
    public class UserEventActivity
    {
        public Guid EventId { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }
}