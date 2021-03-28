using System;

namespace MagicEvents.Api.Service.Application.Auth.interfaces
{
    public interface IJwtSettings
    {
        string Secret { get; set; }
        TimeSpan TokenLifetime { get; set; }
    }
}