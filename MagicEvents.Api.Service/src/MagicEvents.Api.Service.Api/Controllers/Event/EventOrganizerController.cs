using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Api.Common;
using MagicEvents.Api.Service.Application.DTOs.Events.AddCoOrganizer;
using MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.Api.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicEvents.Api.Service.Api.Controllers.Event
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EventOrganizerController : SecuredControllerBase
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
            await _eventOrganizerService.CreateEventAsync(eventId, UserId, createEventDto);
            var locationUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}/Event/{eventId}";
            return Created(locationUrl, null);
        }

        [HttpPost("{eventId}/coorganizers")]
        public async Task<IActionResult> AddCoOrganizer([FromRoute]Guid eventId, [FromBody]AddCoOrganizerDto addCoOrganizerDto)
        {
            await _eventOrganizerService.AddCoOrganizerAsync(eventId, addCoOrganizerDto.UserId, UserId);
            return NoContent();
        }

        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEvent([FromRoute]Guid eventId, [FromBody]UpdateEventDto updateEventDto)
        {
            await _eventOrganizerService.UpdateEventAsync(eventId, UserId, updateEventDto);
            return NoContent();
        }

        [HttpPatch("{eventId}/cancel")]
        public async Task<IActionResult> CancelEvent([FromRoute] Guid eventId)
        {
            await _eventOrganizerService.CancelEventAsync(eventId, UserId);
            return NoContent();
        }

        [HttpPatch("{eventId}/thumbnail")]
        public async Task<IActionResult> SetThumbnail([FromRoute] Guid eventId, [FromForm] IFormFile file)
        {
            //TODO: Add image validation and normalization (required content-type: image/jpeg)
            var binaryData = await FileConverter.ConvertToByteArray(file);
            await _eventOrganizerService.SetThumbnailAsync(eventId, UserId, binaryData);
            return NoContent();
        }
    }
}