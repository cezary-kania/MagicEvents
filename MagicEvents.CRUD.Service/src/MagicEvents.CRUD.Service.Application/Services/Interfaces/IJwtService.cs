using System;
using MagicEvents.CRUD.Service.Application.DTOs.Users.Identity;
using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IJwtService
    {
        AuthTokenDto GenerateToken(User user);
    }
}