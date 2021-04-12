using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginatedResponse;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginationQuery;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventDto> GetEventAsync(Guid id);
        Task<byte[]> GetEventThumbnailAsync(Guid id);
        Task<PaginatedResponse<EventDto>> GetEventsAsync(PaginationQueryDto paginationQuery);
    }
}