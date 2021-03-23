using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Users;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IUserActivityService
    {
        Task<IEnumerable<UserEventActivityDto>> GetActivities(Guid userId);
        Task RegisterEvent(Guid userId, Guid eventId, string userRole);
        Task ChangeRole(Guid userId, Guid eventId, string userRole);
    }
}