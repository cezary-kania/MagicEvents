using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace MagicEvents.CRUD.Service.Api.Controllers.Event
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
        public async Task<IActionResult> GetAllEvents()
            => Ok(await _eventService.GetAllEventsAsync());

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent([FromRoute]Guid eventId)
        {
            var e = await _eventService.GetEventAsync(eventId);
            if(e is null) return NotFound();
            return Ok(e);
        }

        [HttpGet("{eventId}/thumbnail")]
        public async Task<IActionResult> GetThumbnail([FromRoute]Guid eventId)
        {
            var thumbnail = await _eventService.GetEventThumbnailAsync(eventId);
            if(thumbnail is null) return NotFound();
            return File(thumbnail,"image/jpeg",$"event-{eventId}.jpg");
        }
    }
}