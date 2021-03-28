using System.Text;
using FluentValidation.AspNetCore;
using MagicEvents.CRUD.Service.Api.Filters;
using MagicEvents.CRUD.Service.Application;
using MagicEvents.CRUD.Service.Application.Auth.interfaces;
using MagicEvents.CRUD.Service.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace MagicEvents.CRUD.Service.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure(Configuration);
            services.AddApplication(Configuration);
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options => {
                    options.SuppressModelStateInvalidFilter = true;
                });
            services.AddControllersWithViews(options => {
                    options.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MagicEvents.CRUD.Service.Api", Version = "v1" });
            }); 
            var jwtSettings = services.BuildServiceProvider().GetRequiredService<IJwtSettings>();
            services.AddAuthentication("Bearer")
                .AddJwtBearer(cfg => {
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MagicEvents.CRUD.Service.Api v1"));
            }

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
