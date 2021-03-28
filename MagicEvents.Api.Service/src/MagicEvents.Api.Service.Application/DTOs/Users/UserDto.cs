using System;
using System.Collections.Generic;

namespace MagicEvents.Api.Service.Application.DTOs.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public UserIdentityDto Identity { get; set; }
        public UserProfileDto Profile { get; set; }
        public IEnumerable<UserEventActivityDto> EventActivities { get; set; }
    }
}