using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginatedResponse;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginationQuery;
using MagicEvents.Api.Service.Domain.Entities;

namespace MagicEvents.Api.Service.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public EventService(IMapper mapper, IEventRepository eventRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }

        public async Task<PaginatedResponse<EventDto>> GetEventsAsync(PaginationQueryDto paginationQuery)
        {
            var totalEventsNumber = await _eventRepository.CountAsync();
            ValidatePaginationQuery(paginationQuery, totalEventsNumber);
            var events = new List<Event>();
            if(totalEventsNumber > 0)
            {
                var skip = paginationQuery.PageNumber * paginationQuery.PageSize;
                events.AddRange(await _eventRepository.GetAsync(skip, paginationQuery.PageSize));
            }
            var pageSize = paginationQuery.PageSize > totalEventsNumber ? (int) totalEventsNumber : paginationQuery.PageSize;
            var totalPages = (pageSize < 1) ? 0 : (int) Math.Ceiling(totalEventsNumber / (double) pageSize);
            return new PaginatedResponse<EventDto>(
                _mapper.Map<IEnumerable<EventDto>>(events),
                paginationQuery.PageNumber,
                totalPages,
                totalEventsNumber
            );
        }

        public async Task<EventDto> GetEventAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            return _mapper.Map<EventDto>(@event);
        }

        public async Task<EventDto> GetEventAsync(string title)
        {
            var @event = await _eventRepository.GetAsync(title);
            return _mapper.Map<EventDto>(@event);
        }

        public async Task<byte[]> GetEventThumbnailAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            return @event?.Thumbnail?.BinaryData;
        }

        private static void ValidatePaginationQuery(PaginationQueryDto paginationQuery, long totalEventsNumber)
        {
            if (paginationQuery.PageSize < 1)
            {
                throw new ServiceException(ExceptionMessage.Org.InvalidPaginationParams);
            }
            if(totalEventsNumber == 0 && paginationQuery.PageNumber == 0) return;
            if ((int)Math.Ceiling(totalEventsNumber / (double)paginationQuery.PageSize) < paginationQuery.PageNumber + 1)
            {
                throw new ServiceException(ExceptionMessage.Org.InvalidPaginationParams);
            }
        }
    }
}