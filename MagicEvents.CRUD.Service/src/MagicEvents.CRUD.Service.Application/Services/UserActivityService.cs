using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Users;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using MagicEvents.CRUD.Service.Domain.Enums;
using MagicEvents.CRUD.Service.Domain.Repositories;
using MagicEvents.CRUD.Service.Application.Exceptions;
using MagicEvents.CRUD.Service.Domain.Entities;
using System.Linq;

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

        public async Task<IEnumerable<UserEventActivityDto>> GetActivities(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
            return _mapper.Map<IEnumerable<UserEventActivityDto>>(user.EventActivities);
        }

        public async Task RegisterOnEvent(Guid userId, Guid eventId, string userRole)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user is null)
            {
                throw new ServiceException(ExceptionMessage.User.UserNotFound);
            }
            var @event = await _eventRepository.GetAsync(eventId);
            if(@event is null)
            {
                throw new ServiceException(ExceptionMessage.Event.EventNotFound);
            }
            @event.AddParticipant(userId, userRole);
            await _userRepository.UpdateAsync(user);
            await _eventRepository.UpdateAsync(@event);
        }
        
        private void ValidateRole(string role)
        {
            List<string> allowedRoles = typeof(UserEventRole).GetFields()
                .Where(x => x.Name != nameof(UserEventRole.Organizer))
                .Select(x => x.GetValue(null).ToString())
                .ToList();
            if(!allowedRoles.Contains(role))
            {
                throw new ServiceException(ExceptionMessage.Event.InvalidEventRole);
            }
        }
    }
}