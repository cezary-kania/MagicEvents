using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IUserActivityService
    {
        Task<IEnumerable<UserEventActivityDto>> GetActivities(Guid userId);
        Task RegisterOnEvent(Guid userId, Guid eventId, string userRole);
        Task RemoveFromEvent(Guid userId, Guid eventId);
    }
}