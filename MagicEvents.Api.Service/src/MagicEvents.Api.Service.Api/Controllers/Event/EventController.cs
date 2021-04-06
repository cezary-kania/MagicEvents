using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Application.DTOs.Events;
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

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
        public async Task<IActionResult> GetAllEvents()
            => Ok(await _eventService.GetAllEventsAsync());

        [HttpGet("{eventId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetEvent([FromRoute]Guid eventId)
        {
            var e = await _eventService.GetEventAsync(eventId);
            if(e is null) return NotFound();
            return Ok(e);
        }

        [HttpGet("{eventId}/thumbnail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetThumbnail([FromRoute]Guid eventId)
        {
            var thumbnail = await _eventService.GetEventThumbnailAsync(eventId);
            if(thumbnail is null) return NotFound();
            return File(thumbnail,"image/jpeg",$"event-{eventId}.jpg");
        }
    }
}