using System;
using MagicEvents.CRUD.Service.Application.Auth.interfaces;

namespace MagicEvents.CRUD.Service.Services.Auth
{
    public class JwtSettings : IJwtSettings
    {
        public string Secret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
    }
}