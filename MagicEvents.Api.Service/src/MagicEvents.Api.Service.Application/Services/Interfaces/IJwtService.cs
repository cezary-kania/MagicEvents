using System;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity;
using MagicEvents.Api.Service.Domain.Entities;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IJwtService
    {
        AuthTokenDto GenerateToken(User user);
    }
}