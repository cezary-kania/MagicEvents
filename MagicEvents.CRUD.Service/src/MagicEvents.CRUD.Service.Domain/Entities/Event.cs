using System;
using System.Collections.Generic;
using MagicEvents.CRUD.Service.Domain.Enums;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; protected set; }
        public Guid OrganizerId { get; protected set; }
        public EventParticipants Participants { get; protected set; }
        public EventThumbnail Thumbnail { get; protected set; }
        public string Title { get; set; }
        public string Description { get; set;}
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string Status { get; set; }
        
        protected Event(Guid id,
                        Guid organizerId,
                        string title,
                        string description,
                        DateTime startsAt,
                        DateTime endsAt)
        {
            Id = id;
            OrganizerId = organizerId;
            Title = title;
            Description = description;
            StartsAt = startsAt;
            EndsAt = endsAt;
            Status = EventStatus.Open;
            Participants = new EventParticipants();
        }

        public void SetThumbnail(byte[] thData)
        {
            Thumbnail = EventThumbnail.Create(Id, thData);
        }

        public void AddParticipant(Guid userId, string role)
        {
            Participants.AddParticipant(userId, role);
        }

        public void RemoveParticipant(Guid userId) 
        {
            Participants.RemoveParticipant(userId);
        }

        public bool IsOrganizer(Guid userId) 
            => OrganizerId == userId;
        
        public bool IsOpenForRegistration()
            => Status == EventStatus.Open;

        public static Event CreateEvent(Guid id, Guid organizerId, string title, string description, DateTime startsAt, DateTime endsAt)
            => new Event(id, organizerId, title, description, startsAt, endsAt);
    }
}