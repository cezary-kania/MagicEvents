using System;

namespace MagicEvents.CRUD.Service.Application.Auth.interfaces
{
    public interface IJwtSettings
    {
        string Secret { get; set; }
        TimeSpan TokenLifetime { get; set; }
    }
}