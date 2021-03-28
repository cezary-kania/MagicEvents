using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.Infrastructure.MongoDb.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicEvents.Api.Service.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<Event> _events;

        public EventRepository(IMongoDbSettings mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var db = client.GetDatabase(mongoDbSettings.DatabaseName);
            _events = db.GetCollection<Event>(mongoDbSettings.EventsCollectionName);
        }
        public async Task CreateAsync(Event newEvent)
            => await _events.InsertOneAsync(newEvent);

        public async Task DeleteAsync(Guid id)
            => await _events.DeleteOneAsync( e => e.Id == id);

        public async Task<IEnumerable<Event>> GetAllAsync()
            => await _events.AsQueryable().ToListAsync();

        public async Task<Event> GetAsync(Guid id)
            => await _events.AsQueryable().FirstOrDefaultAsync(e => e.Id == id);

        public async Task UpdateAsync(Event updatedEvent)
            => await _events.ReplaceOneAsync(e => e.Id == updatedEvent.Id, updatedEvent);
    }
}