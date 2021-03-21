using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Application.DTOs;
using MagicEvents.CRUD.Service.Application.DTOs.CreateEvent;
using MagicEvents.CRUD.Service.Application.DTOs.UpdateEvent;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace MagicEvents.CRUD.Service.Api.Controllers
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
        
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody]CreateEventDto createEventDto)
        {
            var eventId = Guid.NewGuid();
            await _eventService.CreateEvent(eventId, createEventDto);
            return Created($"/Event/{eventId}", null);
        }

        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEvent([FromRoute]Guid eventId, [FromBody]UpdateEventDto updateEventDto)
        {
            await _eventService.UpdateEvent(eventId, updateEventDto);
            return NoContent();
        }

        [HttpPatch("{eventId}/cancel")]
        public async Task<IActionResult> CancelEvent([FromRoute] Guid eventId)
        {
            await _eventService.CancelEvent(eventId);
            return NoContent();
        }

        [HttpPatch("{eventId}/thumbnail")]
        public async Task<IActionResult> SetThumbnail(
            [FromRoute] Guid eventId, 
            [FromForm] IFormFile file)
        {
            //TODO: Add image validation and normalization (required content-type: image/jpeg)
            var binaryData = await ConvertToByteArray(file);
            await _eventService.SetThumbnail(eventId, binaryData);
            return NoContent();
        }

        private async Task<byte[]> ConvertToByteArray(IFormFile thFile) 
        {
            using (var memoryStream  = new MemoryStream())
            {
                await thFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}