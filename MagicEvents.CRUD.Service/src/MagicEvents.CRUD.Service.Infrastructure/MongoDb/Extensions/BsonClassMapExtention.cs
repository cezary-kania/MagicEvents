using MagicEvents.CRUD.Service.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace MagicEvents.CRUD.Service.Infrastructure.MongoDb.Extensions
{
    public static class BsonClassMapExtention
    {
        public static void AddBsonClassMapping(this IServiceCollection services)
        {
            BsonClassMap.RegisterClassMap<EventParticipants>(map => 
            {
                map.AutoMap();
                map.MapField("_standardParticipants")
                    .SetElementName("StandardParticipants");
                map.MapField("_coOrganizers")
                    .SetElementName("CoOrganizers");
            });
        }
    }
}