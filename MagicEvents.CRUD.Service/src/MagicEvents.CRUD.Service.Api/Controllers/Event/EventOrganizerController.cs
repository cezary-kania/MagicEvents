using System;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Api.Common;
using MagicEvents.CRUD.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.CRUD.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.CRUD.Service.Api.Controllers.Event
{
    public class EventOrganizerController : ControllerBase
    {
        private readonly IEventOrganizerService _eventOrganizerService;
        public EventOrganizerController(IEventOrganizerService eventOrganizerService)
        {
            _eventOrganizerService = eventOrganizerService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody]CreateEventDto createEventDto)
        {
            var eventId = Guid.NewGuid();
            await _eventOrganizerService.CreateEvent(eventId, createEventDto);
            return Created($"/Event/{eventId}", null);
        }

        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEvent([FromRoute]Guid eventId, [FromBody]UpdateEventDto updateEventDto)
        {
            await _eventOrganizerService.UpdateEvent(eventId, updateEventDto);
            return NoContent();
        }

        [HttpPatch("{eventId}/cancel")]
        public async Task<IActionResult> CancelEvent([FromRoute] Guid eventId)
        {
            await _eventOrganizerService.CancelEvent(eventId);
            return NoContent();
        }

        [HttpPatch("{eventId}/thumbnail")]
        public async Task<IActionResult> SetThumbnail([FromRoute] Guid eventId, [FromForm] IFormFile file)
        {
            //TODO: Add image validation and normalization (required content-type: image/jpeg)
            var binaryData = await FileConverter.ConvertToByteArray(file);
            await _eventOrganizerService.SetThumbnail(eventId, binaryData);
            return NoContent();
        }
    }
}