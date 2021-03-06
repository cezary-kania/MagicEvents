using System;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Api.Common;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> CreateEventAsync([FromBody]CreateEventDto createEventDto)
        {
            var eventId = Guid.NewGuid();
            await _eventOrganizerService.CreateEventAsync(eventId, UserId, createEventDto);
            var locationUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}/Event/{eventId}";
            return Created(locationUrl, null);
        }

        [HttpDelete("{eventId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> DeleteEventAsync([FromRoute]Guid eventId)
        {
            await _eventOrganizerService.DeleteEventAsync(eventId, UserId);
            return NoContent();
        }

        [HttpPut("{eventId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> UpdateEventAsync([FromRoute]Guid eventId, [FromBody]UpdateEventDto updateEventDto)
        {
            await _eventOrganizerService.UpdateEventAsync(eventId, UserId, updateEventDto);
            return NoContent();
        }

        [HttpPatch("{eventId}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> CancelEventAsync([FromRoute] Guid eventId)
        {
            await _eventOrganizerService.CancelEventAsync(eventId, UserId);
            return NoContent();
        }

        [HttpPatch("{eventId}/thumbnail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> SetThumbnailAsync([FromRoute] Guid eventId, [FromForm] IFormFile file)
        {
            if(file is null) 
            {
                return BadRequest(new { error = "File can't be null" });
            }
            if(file.IsLargeFile())
            {
                return BadRequest(new {error = "File size limit exceeded"});    
            }
            if(!file.ContainImage())
            {
                return BadRequest(new {error = "File is not image"});
            }
            var binaryData = await file.ToByteArray();
            await _eventOrganizerService.SetThumbnailAsync(eventId, UserId, binaryData);
            return NoContent();
        }
    }
}