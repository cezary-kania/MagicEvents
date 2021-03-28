using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Events;
using MagicEvents.CRUD.Service.Domain.Entities;
using MagicEvents.CRUD.Service.Domain.ValueObjects;

namespace MagicEvents.CRUD.Service.Application.Mappings
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