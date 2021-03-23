using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.CRUD.Service.Application.DTOs.Events.UpdateEvent;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IEventOrganizerService
    {
        Task CancelEvent(Guid id);
        Task SetThumbnail(Guid eventId, byte[] thumbnail);
        Task CreateEvent(Guid eventId, CreateEventDto createEventDto);
        Task UpdateEvent(Guid eventId, UpdateEventDto updateEventDto);
        Task DeleteEvent(Guid id);
    }
}