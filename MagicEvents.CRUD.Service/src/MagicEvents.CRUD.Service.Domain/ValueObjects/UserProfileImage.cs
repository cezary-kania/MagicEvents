using System;

namespace MagicEvents.CRUD.Service.Domain.ValueObjects
{
    public class UserProfileImage
    {
        public Guid UserId { get; set; }
        public byte[] BinaryData { get; set; }       
    }
}