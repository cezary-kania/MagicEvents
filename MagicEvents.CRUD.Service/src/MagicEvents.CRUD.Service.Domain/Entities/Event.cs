using System;
using MagicEvents.CRUD.Service.Domain.Enums;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; protected set; }
        public string Title { get; set; }
        public EventThumbnail Thumbnail { get; protected set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string Status { get; set; }

        protected Event()
        {
        }
        
        protected Event(Guid id, string title, DateTime startsAt, DateTime endsAt)
        {
            Id = id;
            Title = title;
            StartsAt = startsAt;
            EndsAt = EndsAt;
            Status = EventStatus.Open;
        }
        public void SetThumbnail(byte[] thData)
        {
            Thumbnail = EventThumbnail.Create(Id, thData);
        }

        public static Event CreateEvent(Guid id, string title, DateTime startsAt, DateTime endsAt)
            => new Event(id, title, startsAt, endsAt);
    }
}