using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.Infrastructure.MongoDb.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MagicEvents.Api.Service.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IMongoCollection<Event> _events;
        private readonly IMongoDatabase _database;
        private readonly string _eventCollectionName; 

        public EventRepository(IMongoDbSettings mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            _database = client.GetDatabase(mongoDbSettings.DatabaseName);
            _eventCollectionName = mongoDbSettings.EventsCollectionName;
            _events = _database.GetCollection<Event>(mongoDbSettings.EventsCollectionName);
        }
        public async Task CreateAsync(Event newEvent)
            => await _events.InsertOneAsync(newEvent);

        public async Task DeleteAsync(Guid id)
            => await _events.DeleteOneAsync( e => e.Id == id);

        public async Task<IEnumerable<Event>> GetAsync(int skip, int limit)
            => await _events.AsQueryable().Skip(skip).Take(limit).ToListAsync();

        public async Task<Event> GetAsync(Guid id)
            => await _events.AsQueryable().FirstOrDefaultAsync(e => e.Id == id);
        public async Task<Event> GetAsync(string title)
            => await _events.AsQueryable().FirstOrDefaultAsync(e => e.Title.ToUpper() == title.ToUpper());

        public async Task UpdateAsync(Event updatedEvent)
            => await _events.ReplaceOneAsync(e => e.Id == updatedEvent.Id, updatedEvent);

        public async Task DeleteAllAsync()
            => await _database.DropCollectionAsync(_eventCollectionName);

        public async Task<long> CountAsync()
            => await _events.CountDocumentsAsync(new BsonDocument());
    }
}