using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;

namespace MagicEvents.Api.Service.Infrastructure.Repositories.InMemoryRepositories
{
    public class InMemoryEventRepository : IEventRepository
    {
        private ISet<Event> _events = new HashSet<Event>();
        public Task CreateAsync(Event newEvent)
        {
            _events.Add(newEvent);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var @event = _events.SingleOrDefault(x => x.Id == id);
            _events.Remove(@event);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Event>> GetAsync(int skip, int limit)
        {
            return Task.FromResult(_events.Skip(skip).Take(limit).AsEnumerable()); 
        }

        public Task<Event> GetAsync(Guid id)
        {
            return Task.FromResult(_events.SingleOrDefault(x => x.Id == id));
        }

        public Task<long> CountAsync()
            => Task.FromResult((long) _events.Count);

        public Task UpdateAsync(Event updatedEvent)
        {
            var existingEvent = Task.Run(async () => await GetAsync(updatedEvent.Id)).Result;
            _events.Remove(existingEvent);
            _events.Add(updatedEvent);
            return Task.CompletedTask;
        }

        public Task DeleteAllAsync()
        {
            _events.Clear();
            return Task.CompletedTask;
        }

    }
}