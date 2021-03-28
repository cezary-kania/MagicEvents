using System;

namespace MagicEvents.Api.Service.Domain.ValueObjects
{
    public class EventThumbnail
    {
        public Guid EventId { get; protected set; }
        public byte[] BinaryData { get; set; }
        public EventThumbnail(Guid eventId, byte[] binaryData)
        {
            EventId = eventId;
            BinaryData = binaryData;
        }
        public static EventThumbnail Create(Guid eventId, byte[] binaryData)
            => new EventThumbnail(eventId, binaryData);
    }
}