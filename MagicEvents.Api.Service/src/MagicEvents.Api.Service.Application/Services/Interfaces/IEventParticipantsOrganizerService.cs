using System;
using System.Threading.Tasks;

namespace MagicEvents.Api.Service.Application.Services.Interfaces
{
    public interface IEventParticipantsOrganizerService
    {
        Task AddCoOrganizerAsync(Guid eventId, Guid userId, Guid organizerId);
        Task RemoveUserFromEventAsync(Guid eventId, Guid userId, Guid crewUserId);
        Task BanUserOnEventAsync(Guid eventId, Guid userId, Guid crewUserId);
    }
}