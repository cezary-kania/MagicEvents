using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Users;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IUserActivityService
    {
        Task<IEnumerable<UserEventActivityDto>> GetActivitiesAsync(Guid userId);
        Task RegisterOnEventAsync(Guid userId, Guid eventId);
        Task LeaveEventAsync(Guid userId, Guid eventId);
    }
}