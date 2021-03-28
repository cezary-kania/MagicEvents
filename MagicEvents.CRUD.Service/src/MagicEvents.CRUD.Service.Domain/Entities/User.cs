using System;
using System.Collections.Generic;
using System.Linq;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Domain.Entities
{
    public class User
    {
        public Guid Id { get; protected set; }
        public UserIdentity Identity { get; protected set; }
        public UserProfile Profile { get; protected set; }
        public List<UserEventActivity> EventActivities { get; set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public User(Guid id, UserIdentity identity = null, UserProfile profile = null)
        {
            Id = id;
            Identity = identity ?? new UserIdentity();
            Profile = profile ?? new UserProfile();
            EventActivities = new List<UserEventActivity>();
            CreatedAt = UpdatedAt = DateTime.UtcNow;
        }

        public void AddToActivities(Guid eventId, string userRole)
        {
            EventActivities.Add(new UserEventActivity
            {
                EventId = eventId,
                Role = userRole
            });
        }

        public void RemoveActivity(Guid eventId)
        {
            var activity = EventActivities.SingleOrDefault(e => e.EventId == eventId);
            if(activity is null) return;
            EventActivities.Remove(activity);
        }

        public bool IsRegisteredForEvent(Guid eventId)
        {
            return EventActivities
                .SingleOrDefault(x => x.EventId == eventId) is not null;
        }
    }
}