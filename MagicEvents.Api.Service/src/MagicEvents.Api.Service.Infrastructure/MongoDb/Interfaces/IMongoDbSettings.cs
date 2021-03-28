namespace MagicEvents.Api.Service.Infrastructure.MongoDb.Interfaces
{
    public interface IMongoDbSettings
    {
        string EventsCollectionName { get; set; }
        string UsersCollectionName { get; set; } 
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}