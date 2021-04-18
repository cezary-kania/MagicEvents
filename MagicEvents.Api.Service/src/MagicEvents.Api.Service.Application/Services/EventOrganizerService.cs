using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.Api.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Enums;
using MagicEvents.Api.Service.Domain.Repositories;

namespace MagicEvents.Api.Service.Application.Services
{
    public class EventOrganizerService : IEventOrganizerService
    {
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IImageProcessor _imageProcessor;

        public EventOrganizerService(IMapper mapper, IEventRepository eventRepository, IUserRepository userRepository, IImageProcessor imageProcessor)
        {
            _mapper = mapper;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _imageProcessor = imageProcessor;
        }
        public async Task CreateEventAsync(Guid eventId, Guid organizerId, CreateEventDto createEventDto)
        {
            var eventOrganizer = await _userRepository.GetAsync(organizerId);
            if(eventOrganizer is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
            var existingEvent = await _eventRepository.GetAsync(createEventDto.Title);
            if(existingEvent is not null)
            {
                throw new ServiceException(ExceptionMessage.Event.EventAlreadyExist);
            }
            var @event = Event.CreateEvent(
                eventId,
                organizerId, 
                createEventDto.Title,
                createEventDto.Description, 
                createEventDto.StartsAt,
                createEventDto.EndsAt);
            await _eventRepository.CreateAsync(@event);
            eventOrganizer.AddToActivities(eventId, UserEventRole.Organizer, 
                EventActivityStatus.Active);
            await _userRepository.UpdateAsync(eventOrganizer);
        }

        public async Task DeleteEventAsync(Guid id, Guid userId)
        {
            var @event = await _eventRepository.GetAsync(id);
            if (@event is null)
            {
                throw new ServiceException(ExceptionMessage.Event.EventNotFound);
            }
            if (!@event.IsOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }
            await RemoveEventFromActivityLists(userId, @event);
            await _eventRepository.DeleteAsync(@event.Id);
        }

        public async Task CancelEventAsync(Guid id, Guid userId)
        {
            await TryUpdateAsync(id, userId, @event => {
                 @event.Status = EventStatus.Canceled;
            });
        }

        public async Task SetThumbnailAsync(Guid eventId, Guid userId, byte[] thumbnail)
        {
            var @event = await _eventRepository.GetAsync(eventId);
            if(@event is null)
            {
                throw new ServiceException(ExceptionMessage.Event.EventNotFound);
            }
            if(!@event.IsOrganizer(userId) && !@event.Participants.IsCoOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }
            if(!_imageProcessor.IsValidImage(thumbnail))
            {
                throw new ServiceException(ExceptionMessage.File.InvalidInputFile);
            }
            thumbnail = await _imageProcessor.CreateThumbnailAsync(thumbnail);
            @event.SetThumbnail(thumbnail);
            await _eventRepository.UpdateAsync(@event);
        }
        public async Task UpdateEventAsync(Guid eventId, Guid userId, UpdateEventDto updateEventDto)
        {
            await TryUpdateAsync(eventId, userId, @event => {
                 @event.Title = updateEventDto.Title;
                 @event.Description = updateEventDto.Description;
                 @event.StartsAt = updateEventDto.StartsAt;
                 @event.EndsAt = updateEventDto.EndsAt;
            });
        } 

        private async Task TryUpdateAsync(Guid eventId, Guid userId, Action<Event> updateAction)
        {
            var @event = await _eventRepository.GetAsync(eventId);
            if(@event is null)
            {
                throw new ServiceException(ExceptionMessage.Event.EventNotFound);
            }
            if(!@event.IsOrganizer(userId) && !@event.Participants.IsCoOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }
            updateAction(@event);
            await _eventRepository.UpdateAsync(@event);
        }

        private async Task<User> TryGetUser(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
            return user;
        }

        private async Task<Event> TryGetEvent(Guid eventId)
        {
            var @event = await _eventRepository.GetAsync(eventId);
            if (@event is null)
            {
                throw new ServiceException(ExceptionMessage.Event.EventNotFound);
            }
            return @event;
        }

        private async Task RemoveEventFromActivityLists(Guid userId, Event @event)
        {
            List<Guid> allParticipantsIds = @event.Participants.StandardParticipants.ToList();
            allParticipantsIds.AddRange(@event.Participants.CoOrganizers);
            List<User> usersToUpdate = new List<User>();
            foreach (Guid participantId in allParticipantsIds)
            {
                var participant = await _userRepository.GetAsync(userId);
                if (participant is null)
                {
                    throw new ServiceException(ExceptionMessage.Event.UserNotRegisteredForEvent);
                }
                participant.RemoveActivity(@event.Id);
            }
            await _userRepository.UpdateAsync(usersToUpdate);
        }
    }
}