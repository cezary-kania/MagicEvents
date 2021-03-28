using System;

namespace MagicEvents.Api.Service.Application.DTOs.Users
{
    public class UserEventActivityDto
    {
        public Guid EventId { get; set; }
        public string Role { get; set; }
    }
}