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
    public class EventParticipantsOrganizerController : SecuredControllerBase
    {
        private readonly IEventParticipantsOrganizerService _eventParticipantsOrganizerService;
        public EventParticipantsOrganizerController(IEventParticipantsOrganizerService eventParticipantsOrganizerService)
        {
            _eventParticipantsOrganizerService = eventParticipantsOrganizerService;
        }

        [HttpPost("{eventId}/coorganizers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> AddCoOrganizerAsync([FromRoute]Guid eventId, [FromBody]AddCoOrganizerDto addCoOrganizerDto)
        {
            await _eventParticipantsOrganizerService.AddCoOrganizerAsync(eventId, addCoOrganizerDto.UserId, UserId);
            return NoContent();
        }

        [HttpDelete("{eventId}/coorganizers/{coOrganizerId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> RemoveCoOrganizerAsync([FromRoute]Guid eventId, [FromRoute]Guid coOrganizerId)
        {
            await _eventParticipantsOrganizerService.RemoveCoOrganizerAsync(eventId, coOrganizerId, UserId);
            return NoContent();
        }

        [HttpDelete("{eventId}/participants/{participantId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> RemoveParticipantAsync([FromRoute]Guid eventId, [FromRoute]Guid participantId)
        {
            await _eventParticipantsOrganizerService.RemoveUserFromEventAsync(eventId, participantId, UserId);
            return NoContent();
        }

        [HttpPatch("{eventId}/participants/{participantId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object),400)]
        public async Task<IActionResult> BanParticipantAsync([FromRoute]Guid eventId, [FromRoute]Guid participantId)
        {
            await _eventParticipantsOrganizerService.BanUserOnEventAsync(eventId, participantId, UserId);
            return NoContent();
        }
    }
}