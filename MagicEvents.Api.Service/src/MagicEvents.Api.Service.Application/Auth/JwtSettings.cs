using System;
using MagicEvents.Api.Service.Application.Auth.interfaces;

namespace MagicEvents.Api.Service.Services.Auth
{
    public class JwtSettings : IJwtSettings
    {
        public string Secret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
    }
}