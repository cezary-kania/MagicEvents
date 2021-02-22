using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.CRUD.Service.Domain.Entities;

namespace MagicEvents.CRUD.Service.Domain.Repositories
{
    public interface IEventRepository
    {
        Task<Event> GetAsync(Guid id);
        Task<IEnumerable<Event>> GetAllAsync();
        Task CreateAsync(Event newEvent);
        Task UpdateAsync(Event updatedEvent);
        Task DeleteAsync(Guid id);
    }
}