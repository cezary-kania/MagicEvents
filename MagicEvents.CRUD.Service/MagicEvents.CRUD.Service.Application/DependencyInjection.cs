using System.Reflection;
using MagicEvents.CRUD.Service.Application.Services;
using MagicEvents.CRUD.Service.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MagicEvents.CRUD.Service.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IEventService, EventService>();
            return services;
        }
    }
}