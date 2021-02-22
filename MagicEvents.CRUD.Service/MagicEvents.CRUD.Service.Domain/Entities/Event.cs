using System;
using MagicEvents.CRUD.Service.Domain.Enums;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; protected set; }
        public string Title { get; protected set; }
        public EventThumbnail Thumbnail { get; protected set; }
        public DateTime StartsAt { get; protected set; }
        public DateTime EndsAt { get; protected set; }
        public string Status { get; set; }

        protected Event()
        {
        }
        
        protected Event(Guid id, string title, DateTime startsAt, DateTime endsAt)
        {
            Id = id;
            SetTitle(title);
            SetEventTime(startsAt, endsAt);
            Status = EventStatus.Open;
        }

        public void SetTitle(string title)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                throw new Exception("Title can not be empty.");
            }
            Title = title;
        }
        public void SetEventTime(DateTime startDate, DateTime endDate)
        {
            if(startDate < DateTime.UtcNow ||
                endDate < DateTime.UtcNow 
                || startDate > endDate)
            {
                throw new Exception($"Invalid event time: {startDate} - {endDate}");
            }
            StartsAt = startDate;
            EndsAt = endDate;
        }
        public void SetThumbnail(byte[] thData)
        {
            Thumbnail = EventThumbnail.Create(Id, thData);
        }

        public static Event CreateEvent(Guid id, string title, DateTime startsAt, DateTime endsAt)
            => new Event(id, title, startsAt, endsAt);
    }
}