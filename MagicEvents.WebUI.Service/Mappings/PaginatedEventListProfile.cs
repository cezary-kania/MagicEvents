using AutoMapper;
using MagicEvents.WebUI.Service.DTOs.Events;
using MagicEvents.WebUI.Service.DTOs.Pagination;
using MagicEvents.WebUI.Service.Models.Events;

namespace MagicEvents.WebUI.Service.Mappings
{
    public class PaginatedEventListProfile : Profile
    {
        public PaginatedEventListProfile()
        {
            CreateMap<EventDto, EventViewModel>()
                .ForMember(x => x.Organizer, 
                    opt => opt.MapFrom(x => x.OrganizerId));
            CreateMap<PaginatedResponseDto<EventDto>,PaginatedListViewModel<EventViewModel>>();
        }
    }
}