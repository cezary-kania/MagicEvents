using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs.Events;
using MagicEvents.CRUD.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.CRUD.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Application.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventDto> GetEvent(Guid id);
        Task<byte[]> GetEventThumbnail(Guid id);
        Task<IEnumerable<EventDto>> GetAllEvents();
    }
}