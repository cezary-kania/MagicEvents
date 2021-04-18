using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Domain.Entities;

namespace MagicEvents.Api.Service.Domain.Repositories
{
    public interface IEventRepository
    {
        Task<Event> GetAsync(Guid id);
        Task<Event> GetAsync(string title);
        Task<IEnumerable<Event>> GetAsync(int skip, int limit);
        Task<long> CountAsync();
        Task CreateAsync(Event newEvent);
        Task UpdateAsync(Event updatedEvent);
        Task DeleteAsync(Guid id);
        Task DeleteAllAsync();
    }
}