using AutoMapper;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.ValueObjects;

namespace MagicEvents.Api.Service.Application.Mappings
{
    public class EventMapProfile : Profile
    {
        public EventMapProfile()
        {
            CreateMap<Event,EventDto>();
            CreateMap<EventParticipants,EventParticipantsDto>();
        }
    }
}