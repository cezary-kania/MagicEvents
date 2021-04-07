using System;
using System.Collections.Generic;
using System.Linq;
using MagicEvents.Api.Service.Domain.Enums;
using MagicEvents.Api.Service.Domain.Exceptions;
using MagicEvents.Api.Service.Domain.ValueObjects;

namespace MagicEvents.Api.Service.Domain.Entities
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

        public void AddToActivities(Guid eventId, string userRole, string activityStatus)
        {
            EventActivities.Add(new UserEventActivity
            {
                EventId = eventId,
                Role = userRole,
                Status = activityStatus
            });
        }

        public void ChangeActivityStatus(Guid eventId, string activityStatus)
        {
            var activity = EventActivities.SingleOrDefault(e => e.EventId == eventId);
            if(activity is null)
            {
                throw new DomainException(DomainExceptionMessage.Event.UserNotRegisteredForEvent);
            }
            activity.Status = activityStatus;
        }
        public void RemoveActivity(Guid eventId)
        {
            var activity = EventActivities.SingleOrDefault(e => e.EventId == eventId);
            if(activity is null) return;
            EventActivities.Remove(activity);
        }

        public bool IsRegisteredOnEvent(Guid eventId)
        {
            return EventActivities
                .SingleOrDefault(x => x.EventId == eventId) is not null;
        }

        public bool CanRegisterOnEvent(Guid eventId)
        {
            var eventActivity = EventActivities
                .SingleOrDefault(x => x.EventId == eventId);
            if(eventActivity is null) return true;
            if(eventActivity.Status == EventActivityStatus.Left)
            {
                return true;
            }
            return false;
        }
    }
}