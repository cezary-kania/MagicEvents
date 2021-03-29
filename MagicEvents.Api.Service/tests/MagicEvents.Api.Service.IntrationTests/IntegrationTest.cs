using System;
using System.Net.Http;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Api;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.Infrastructure.Repositories;
using MagicEvents.Api.Service.IntrationTests.InMemoryRepositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MagicEvents.Api.Service.IntrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient TestClient;
        private readonly IServiceProvider _serviceProvider;
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => 
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(IEventRepository));
                        services.TryAddScoped<IEventRepository, InMemoryEventRepository>();
                    });
                });
            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }

        public void Dispose()
        {
            var serviceScope = _serviceProvider.CreateScope();
            var repository = serviceScope.ServiceProvider.GetService<IEventRepository>();
            var events = Task.Run(repository.GetAllAsync).Result;
            foreach(var @event in events)
            {
                Task.Run(async () => await repository.DeleteAsync(@event.Id));
            }
        }
    }
}