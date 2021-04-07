using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.Api.Service.Application.DTOs.Events.UpdateEvent;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IEventOrganizerService
    {
        Task CancelEventAsync(Guid id, Guid userId);
        Task SetThumbnailAsync(Guid eventId, Guid userId, byte[] thumbnail);
        Task CreateEventAsync(Guid eventId, Guid organizerId, CreateEventDto createEventDto);
        Task UpdateEventAsync(Guid eventId, Guid userId, UpdateEventDto updateEventDto);
        Task DeleteEventAsync(Guid id, Guid userId);
        Task AddCoOrganizerAsync(Guid eventId, Guid userId, Guid organizerId);
        Task RemoveUserFromEventAsync(Guid eventId, Guid userId, Guid crewUserId);
        Task BanUserOnEventAsync(Guid eventId, Guid userId, Guid crewUserId);
    }
}