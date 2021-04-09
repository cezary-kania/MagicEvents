using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Enums;
using MagicEvents.Api.Service.Domain.Repositories;

namespace MagicEvents.Api.Service.Application.Services
{
    public class EventParticipantsOrganizerService : IEventParticipantsOrganizerService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        public EventParticipantsOrganizerService(IEventRepository eventRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
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

            if(@event.Participants.IsStandardParticipant(coOrganizerId))
            {
                @event.Participants.RemoveParticipant(coOrganizerId);
                coOrganizer.RemoveActivity(eventId);     
            }
            
            coOrganizer.AddToActivities(eventId, UserEventRole.CoOrganizer, 
                EventActivityStatus.Active);
            @event.AddParticipant(coOrganizerId, UserEventRole.CoOrganizer);
            await _userRepository.UpdateAsync(coOrganizer);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task BanUserOnEventAsync(Guid eventId, Guid userId, Guid crewUserId)
        {
            var user = await TryGetUser(userId);
            var @event = await TryGetEvent(eventId);
            if(!@event.IsOrganizer(crewUserId) && !@event.IsCoOrganizer(crewUserId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }
            if(@event.IsOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.Event.OrgCantLeaveEvent);
            }
            if (!user.IsRegisteredOnEvent(eventId))
            {
                throw new ServiceException(ExceptionMessage.Event.UserNotRegisteredForEvent);
            }

            user.ChangeActivityStatus(eventId, EventActivityStatus.Banned);
            @event.RemoveParticipant(userId);
            await _userRepository.UpdateAsync(user);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task RemoveUserFromEventAsync(Guid eventId, Guid userId, Guid crewUserId)
        {
            var user = await TryGetUser(userId);
            var @event = await TryGetEvent(eventId);
            if(!@event.IsOrganizer(crewUserId) && !@event.IsCoOrganizer(crewUserId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }
            if(@event.IsOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.Event.OrgCantLeaveEvent);
            }
            if (!user.IsRegisteredOnEvent(eventId))
            {
                throw new ServiceException(ExceptionMessage.Event.UserNotRegisteredForEvent);
            }
            user.RemoveActivity(eventId);
            @event.RemoveParticipant(userId);
            await _userRepository.UpdateAsync(user);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task RemoveCoOrganizerAsync(Guid eventId, Guid coOrganizerId, Guid userId)
        {
            var user = await TryGetUser(coOrganizerId);
            var @event = await TryGetEvent(eventId);
            if(!@event.IsOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.User.NoPermissionForOp);
            }
            if (!user.IsRegisteredOnEvent(eventId))
            {
                throw new ServiceException(ExceptionMessage.Event.UserNotRegisteredForEvent);
            }
            if(!@event.IsCoOrganizer(coOrganizerId))
            {
                throw new ServiceException(ExceptionMessage.Event.InvalidEventRole);
            }
            
            @event.Participants.RemoveParticipant(coOrganizerId);
            user.RemoveActivity(eventId);
            
            user.AddToActivities(eventId, UserEventRole.StandardParticipant,
                EventActivityStatus.Active);
            @event.AddParticipant(coOrganizerId, UserEventRole.StandardParticipant);
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