using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using MagicEvents.WebUI.Service.DTOs.Events;
using MagicEvents.WebUI.Service.DTOs.Pagination;
using MagicEvents.WebUI.Service.Models.Events;
using MagicEvents.WebUI.Service.Services.Interfaces;
using MagicEvents.WebUI.Service.Settings;
using Microsoft.Extensions.Options;

namespace MagicEvents.WebUI.Service.Services
{
    public class EventService : ApiServiceBase, IEventService
    {
        private const string _apiController = "Event";
        private readonly IMapper _mapper;
        public EventService(HttpClient httpClient, IOptions<ApiService> options, IMapper mapper)
            : base(httpClient, options)
        {
            _mapper = mapper;
        }

        public async Task<PaginatedListViewModel<EventViewModel>> GetEvents(int pageIndex, int pageSize)
        {
            var url = $"{_apiBaseUrl}/{_apiController}/all?PageNumber={pageIndex}&PageSize={pageSize}";
            var paginatedEventList = await _httpClient.GetFromJsonAsync<PaginatedResponseDto<EventDto>>(url);
            return _mapper.Map<PaginatedResponseDto<EventDto>,PaginatedListViewModel<EventViewModel>>(paginatedEventList);
        }
    }
}