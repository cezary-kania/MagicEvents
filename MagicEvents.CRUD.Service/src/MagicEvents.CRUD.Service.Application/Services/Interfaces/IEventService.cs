using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs;
using MagicEvents.CRUD.Service.Application.DTOs.CreateEvent;
using MagicEvents.CRUD.Service.Application.DTOs.UpdateEvent;
using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventDto> GetEvent(Guid id);
        Task<byte[]> GetEventThumbnail(Guid id);
        Task<IEnumerable<EventDto>> GetAllEvents();
        Task CancelEvent(Guid id);
        Task SetThumbnail(Guid eventId, byte[] thumbnail);
        Task CreateEvent(Guid eventId, CreateEventDto createEventDto);
        Task UpdateEvent(Guid eventId, UpdateEventDto updateEventDto);
        Task DeleteEvent(Guid id);
    }
}