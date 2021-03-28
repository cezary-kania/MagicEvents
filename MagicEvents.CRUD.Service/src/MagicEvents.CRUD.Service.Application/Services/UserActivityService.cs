using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Users;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using MagicEvents.CRUD.Service.Domain.Enums;
using MagicEvents.CRUD.Service.Domain.Repositories;
using MagicEvents.CRUD.Service.Application.Exceptions;
using System.Linq;
using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Application.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        public UserActivityService(IMapper mapper,
                                   IUserRepository userRepository,
                                   IEventRepository eventRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<UserEventActivityDto>> GetActivitiesAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
            return _mapper.Map<IEnumerable<UserEventActivityDto>>(user.EventActivities);
        }

        public async Task RegisterOnEventAsync(Guid userId, Guid eventId)
        {
            var user = await TryGetUser(userId);
            var @event = await TryGetEvent(eventId);

            if (user.IsRegisteredForEvent(eventId) 
                || @event.IsOrganizer(userId)
                || @event.Participants.IsCoOrganizer(userId)
                || @event.Participants.IsStandardParticipant(userId))
            {
                throw new ServiceException(ExceptionMessage.Event.UserAlreadyRegisteredForEvent);
            }

            user.AddToActivities(eventId, UserEventRole.StandardParticipant);
            @event.AddParticipant(userId, UserEventRole.StandardParticipant);
            await _userRepository.UpdateAsync(user);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task LeaveEventAsync(Guid userId, Guid eventId)
        {
            //TODO: Change removing to shadow leaving
            var user = await TryGetUser(userId);
            var @event = await TryGetEvent(eventId);
            if(@event.IsOrganizer(userId))
            {
                throw new ServiceException(ExceptionMessage.Event.OrgCantLeaveEvent);
            }
            if (!user.IsRegisteredForEvent(eventId))
            {
                throw new ServiceException(ExceptionMessage.Event.UserNotRegisteredForEvent);
            }
            user.RemoveActivity(eventId);
            @event.RemoveParticipant(userId);
            await _userRepository.UpdateAsync(user);
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