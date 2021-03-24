using MagicEvents.CRUD.Service.Infrastructure.MongoDb.Interfaces;

namespace MagicEvents.CRUD.Service.Infrastructure.MongoDb
{
    public class MongoDbSettings : IMongoDbSettings
    {
        public string EventsCollectionName { get; set; } 
        public string UsersCollectionName { get; set; } 
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}