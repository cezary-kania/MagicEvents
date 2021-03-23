using AutoMapper;
using MagicEvents.CRUD.Service.Application.DTOs.Events;
using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Application.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event,EventDto>();
        }
    }
}