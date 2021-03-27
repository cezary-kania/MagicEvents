using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Events;
using MagicEvents.CRUD.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.CRUD.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using MagicEvents.CRUD.Service.Domain.Repositories;
using MagicEvents.CRUD.Service.Domain.Entities;
using MagicEvents.CRUD.Service.Domain.Enums;

namespace MagicEvents.CRUD.Service.Application.Services
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

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto> GetEventAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            if(@event is null)
            {
                throw new Exception($"Event with id: '{id}' does not exist.");
            }
            return _mapper.Map<EventDto>(@event);
        }

        public async Task<byte[]> GetEventThumbnailAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            if(@event is null)
            {
                throw new Exception($"Event with id: '{id}' does not exist.");
            }
            return @event.Thumbnail.BinaryData;
        }
    }
}