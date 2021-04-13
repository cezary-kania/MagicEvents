using AutoMapper;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginatedResponse;

namespace MagicEvents.Api.Service.Application.Mappings
{
    public class PaginatedResponseMapProfile : Profile
    {
        public PaginatedResponseMapProfile()
        {
            CreateMap<PaginatedResponse<EventDto>,PaginatedResponseDto<EventDto>>();
        }
    }
}