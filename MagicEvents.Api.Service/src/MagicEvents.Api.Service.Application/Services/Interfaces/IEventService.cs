using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.Api.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.Api.Service.Domain.Entities;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventDto> GetEventAsync(Guid id);
        Task<byte[]> GetEventThumbnailAsync(Guid id);
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
    }
}