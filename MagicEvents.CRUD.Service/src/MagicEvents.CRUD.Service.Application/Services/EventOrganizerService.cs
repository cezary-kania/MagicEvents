using System;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.CRUD.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.CRUD.Service.Application.Exceptions;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using MagicEvents.CRUD.Service.Domain.Entities;
using MagicEvents.CRUD.Service.Domain.Enums;
using MagicEvents.CRUD.Service.Domain.Repositories;

namespace MagicEvents.CRUD.Service.Application.Services
{
    public class EventOrganizerService : IEventOrganizerService
    {
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;

        public EventOrganizerService(IMapper mapper, IEventRepository eventRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }
        public async Task CreateEventAsync(Guid eventId, Guid organizerId, CreateEventDto createEventDto)
        {
            var eventOrganizer = await _userRepository.GetAsync(organizerId);
            if(eventOrganizer is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
            var @event = Event.CreateEvent(
                eventId,
                organizerId, 
                createEventDto.Title,
                createEventDto.Description, 
                createEventDto.StartsAt,
                createEventDto.EndsAt);
            await _eventRepository.CreateAsync(@event);
            eventOrganizer.AddToActivities(eventId, UserEventRole.Organizer);
            await _userRepository.UpdateAsync(eventOrganizer);
        }

        public async Task AddCoOrganizerAsync(Guid eventId, Guid coOrganizerId, Guid organizerId)
        {
            var organizer = await TryGetUser(organizerId);
            var coOrganizer = await TryGetUser(coOrganizerId);
            var @event = await TryGetEvent(eventId);
            if(!@event.IsOrganizer(organizerId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }

            if(@event.IsOrganizer(coOrganizerId) || @event.Participants.IsCoOrganizer(coOrganizerId))
            {
                throw new ServiceException(ExceptionMessage.Event.UserAlreadyRegisteredForEvent);
            }

            if(!coOrganizer.IsRegisteredForEvent(eventId))
            {
                throw new ServiceException(ExceptionMessage.Org.UknownError);
            }

            if(@event.Participants.IsStandardParticipant(coOrganizerId))
            {
                @event.Participants.RemoveParticipant(coOrganizerId);     
            }
            
            coOrganizer.AddToActivities(eventId, UserEventRole.CoOrganizer);
            @event.AddParticipant(coOrganizerId, UserEventRole.CoOrganizer);
            await _userRepository.UpdateAsync(coOrganizer);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task RemoveUserFromEventAsync(Guid eventId, Guid userId)
        {
            var user = await TryGetUser(userId);
            var @event = await TryGetEvent(eventId);
            
            if (!user.IsRegisteredForEvent(eventId) && !@event.IsOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.Event.UserNotRegisteredForEvent);
            }

            user.RemoveActivity(eventId);
            @event.RemoveParticipant(userId);
            await _userRepository.UpdateAsync(user);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task DeleteEventAsync(Guid id, Guid userId)
        {
            var @event = await _eventRepository.GetAsync(id);
            if(@event is null)
            {
                throw new ServiceException(ExceptionMessage.Event.EventNotFound);
            }
            if(!@event.IsOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }
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
            await TryUpdateAsync(eventId, userId, @event => {
                 @event.SetThumbnail(thumbnail);
            });
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
            if(!@event.IsOrganizer(userId) && !@event.Participants.IsCoOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }
            if(@event is null)
            {
                throw new ServiceException(ExceptionMessage.Event.EventNotFound);
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
    }
}