using System;
using System.Collections.Generic;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Domain.Entities
{
    public class User
    {
        public Guid Id { get; protected set; }
        public UserIdentity Identity { get; set; }
        public UserProfile Profile { get; set; }
        public List<EventActivity> EventActivities { get; set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public User(Guid id)
        {
            Id = id;
            CreatedAt = UpdatedAt = DateTime.UtcNow;
        }
    }
}