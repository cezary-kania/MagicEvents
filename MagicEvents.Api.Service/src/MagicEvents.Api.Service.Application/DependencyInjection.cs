using System.Reflection;
using FluentValidation;
using MagicEvents.Api.Service.Application.Auth.interfaces;
using MagicEvents.Api.Service.Application.Services;
using MagicEvents.Api.Service.Application.Services.Interfaces;
using MagicEvents.Api.Service.Services.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MagicEvents.Api.Service.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services,
                                                        IConfiguration configuration)
        {
            services.Configure<JwtSettings>(
                configuration.GetSection(nameof(JwtSettings)));
            services.AddSingleton<IJwtSettings>(sp => 
                sp.GetRequiredService<IOptions<JwtSettings>>().Value);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<IEventService,EventService>();
            services.AddScoped<IEventOrganizerService,EventOrganizerService>();
            services.AddScoped<IUserActivityService,UserActivityService>();
            services.AddScoped<IUserIdentityService,UserIdentityService>();
            services.AddScoped<IUserProfileService,UserProfileService>();
            services.AddScoped<IUserService,UserService>();
            services.AddSingleton<IEncryptService,EncryptService>();
            services.AddSingleton<IJwtService,JwtService>();
            return services;
        }
    }
}