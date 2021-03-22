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

        public async Task<IEnumerable<EventDto>> GetAllEvents()
        {
            var events = await _eventRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto> GetEvent(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            if(@event is null)
            {
                throw new Exception($"Event with id: '{id}' does not exist.");
            }
            return _mapper.Map<EventDto>(@event);
        }

        public async Task<byte[]> GetEventThumbnail(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            if(@event is null)
            {
                throw new Exception($"Event with id: '{id}' does not exist.");
            }
            return @event.Thumbnail.BinaryData;
        }

        public async Task CreateEvent(Guid eventId, CreateEventDto createEventDto)
        {
            var organizerId = createEventDto.OrganizerId;
            var eventOrganizer = _userRepository.GetAsync(organizerId);
            if(eventOrganizer is null)
            {
                throw new Exception($"Invalid organizer Id");
            }
            var @event = Event.CreateEvent(
                eventId,
                organizerId, 
                createEventDto.Title,
                createEventDto.Description, 
                createEventDto.StartsAt,
                createEventDto.EndsAt);
            await _eventRepository.CreateAsync(@event);
        }

        public async Task DeleteEvent(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);
            if(@event is null)
            {
                throw new Exception($"Event with id: '{id}' does not exist.");
            }
            await _eventRepository.DeleteAsync(@event.Id);
        }
        public async Task CancelEvent(Guid id)
        {
            await TryUpdateAsync(id, @event => {
                 @event.Status = EventStatus.Canceled;
            });
        }

        public async Task SetThumbnail(Guid eventId, byte[] thumbnail)
        {
            await TryUpdateAsync(eventId, @event => {
                 @event.SetThumbnail(thumbnail);
            });
        }
        public async Task UpdateEvent(Guid eventId, UpdateEventDto updateEventDto)
        {
            await TryUpdateAsync(eventId, @event => {
                 @event.Title = updateEventDto.Title;
                 @event.Description = updateEventDto.Description;
                 @event.StartsAt = updateEventDto.StartsAt;
                 @event.EndsAt = updateEventDto.EndsAt;
            });
        } 

        private async Task TryUpdateAsync(Guid eventId, Action<Event> updateAction)
        {
            var @event = await _eventRepository.GetAsync(eventId);
            if(@event is null)
            {
                throw new Exception($"Event with id: '{eventId}' does not exist.");
            }
            updateAction(@event);
            await _eventRepository.UpdateAsync(@event);
        }
    }
}