using MagicEvents.CRUD.Service.Domain.Repositories;
using MagicEvents.CRUD.Service.Infrastructure.MongoDb;
using MagicEvents.CRUD.Service.Infrastructure.MongoDb.Extensions;
using MagicEvents.CRUD.Service.Infrastructure.MongoDb.Interfaces;
using MagicEvents.CRUD.Service.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace MagicEvents.CRUD.Service.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(
                configuration.GetSection(nameof(MongoDbSettings)));
            services.AddSingleton<IMongoDbSettings>(sp => 
                sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
            services.AddScoped<IEventRepository,EventRepository>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddBsonClassMapping();
            return services;
        }
    }
}