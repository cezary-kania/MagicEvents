using System;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.Infrastructure.MongoDb;
using MagicEvents.Api.Service.Infrastructure.MongoDb.Extensions;
using MagicEvents.Api.Service.Infrastructure.MongoDb.Interfaces;
using MagicEvents.Api.Service.Infrastructure.Repositories;
using MagicEvents.Api.Service.Infrastructure.Repositories.InMemoryRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace MagicEvents.Api.Service.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var heathChecksBuilder = services.AddHealthChecks();
            var useMemoryDb = Convert.ToBoolean(configuration.GetSection("UseInMemoryDatabase").Value);
            if(!useMemoryDb)
            {
                services.Configure<MongoDbSettings>(
                    configuration.GetSection(nameof(MongoDbSettings)));
                services.AddSingleton<IMongoDbSettings>(sp => 
                    sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);    
                services.AddBsonClassMapping();
                services.AddScoped<IEventRepository,EventRepository>();
                services.AddScoped<IUserRepository,UserRepository>();

                var mongoDbSettings = services.BuildServiceProvider().GetRequiredService<IMongoDbSettings>(); 
                
                    heathChecksBuilder.AddMongoDb(
                        mongoDbSettings.ConnectionString,
                        name: "mongodb",
                        timeout: TimeSpan.FromSeconds(3)
                    );
            }
            else
            {
                services.AddSingleton<IEventRepository, InMemoryEventRepository>();
                services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            }
            return services;
        }
    }
}