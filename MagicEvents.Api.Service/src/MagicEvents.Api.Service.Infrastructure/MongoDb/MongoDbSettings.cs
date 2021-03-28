using MagicEvents.Api.Service.Infrastructure.MongoDb.Interfaces;

namespace MagicEvents.Api.Service.Infrastructure.MongoDb
{
    public class MongoDbSettings : IMongoDbSettings
    {
        public string EventsCollectionName { get; set; } 
        public string UsersCollectionName { get; set; } 
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}