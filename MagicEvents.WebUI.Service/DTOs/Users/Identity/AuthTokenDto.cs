using System;

namespace MagicEvents.WebUI.Service.DTOs.Users.Identity
{
    public class AuthTokenDto
    {
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}