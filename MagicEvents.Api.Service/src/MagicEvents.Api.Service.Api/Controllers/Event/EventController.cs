using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Api.SwaggerExamples.Responses;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginatedResponse;
using MagicEvents.Api.Service.Application.DTOs.Pagination.PaginationQuery;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace MagicEvents.Api.Service.Api.Controllers.Event
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("all")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponseDto<EventDto>), 200)]
        public async Task<IActionResult> GetAllEventsAsync([FromQuery]PaginationQueryDto paginationQuery)
        {
            if(paginationQuery is null) paginationQuery = new PaginationQueryDto();
            return Ok(await _eventService.GetEventsAsync(paginationQuery));
        }

        [HttpGet("{eventId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetEventAsync([FromRoute]Guid eventId)
        {
            var @event = await _eventService.GetEventAsync(eventId);
            if(@event is null) return NotFound();
            return Ok(@event);
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetEventByTitleAsync([FromQuery]string eventTitle)
        {
            if(eventTitle is null) return BadRequest();
            var @event = await _eventService.GetEventAsync(eventTitle);
            if(@event is null) return NotFound();
            return Ok(@event);
        }

        [HttpGet("{eventId}/thumbnail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetThumbnailAsync([FromRoute]Guid eventId)
        {
            var thumbnail = await _eventService.GetEventThumbnailAsync(eventId);
            if(thumbnail is null) return NotFound();
            return File(thumbnail,"image/jpeg",$"event-{eventId}.jpg");
        }
    }
}