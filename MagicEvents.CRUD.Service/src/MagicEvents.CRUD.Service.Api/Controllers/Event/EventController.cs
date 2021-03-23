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
            => Ok(await _eventService.GetAllEvents());

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent([FromRoute]Guid eventId)
        {
            var e = await _eventService.GetEvent(eventId);
            if(e is null) return NotFound();
            return Ok(e);
        }

        [HttpGet("{eventId}/thumbnail")]
        public async Task<IActionResult> GetThumbnail([FromRoute]Guid eventId)
        {
            var thumbnail = await _eventService.GetEventThumbnail(eventId);
            if(thumbnail is null) return NotFound();
            string thData = Convert.ToBase64String(thumbnail);
            thData = $"data:image/jpg;base64,{thData}";
            return File(thumbnail,"image/jpeg",$"event-{eventId}.jpg");
            //return Ok(new {thSrc = thData}); // There're two different ways to send data - It has to be considered. 
        }
    }
}