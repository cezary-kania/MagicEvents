using System;

namespace MagicEvents.CRUD.Service.Application.DTOs.Users.Identity
{
    public class AuthTokenDto
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}