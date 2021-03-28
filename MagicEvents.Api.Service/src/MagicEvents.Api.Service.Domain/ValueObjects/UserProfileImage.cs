using System;

namespace MagicEvents.Api.Service.Domain.ValueObjects
{
    public class UserProfileImage
    {
        public Guid UserId { get; set; }
        public byte[] BinaryData { get; set; }       
    }
}