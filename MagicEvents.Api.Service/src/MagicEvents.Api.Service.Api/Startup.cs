using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using FluentValidation.AspNetCore;
using MagicEvents.Api.Service.Api.Filters;
using MagicEvents.Api.Service.Application;
using MagicEvents.Api.Service.Application.Auth.interfaces;
using MagicEvents.Api.Service.Application.Exceptions;
using MagicEvents.Api.Service.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace MagicEvents.Api.Service.Api
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

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "MagicEvents.Api", 
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Cezary Kania",
                        Email = "cezary.kaniaq@gmail.com",
                        Url = new Uri("https://cezary-kania.github.io")
                    }
                });

                x.ExampleFilters();

                x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme 
                        {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                        },
                        new List<string>()
                    }
                });

            });
            services.AddSwaggerExamplesFromAssemblyOf<Startup>(); 
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
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MagicEvents.Api v1"));
            
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;
                
                var statusCode = HttpStatusCode.InternalServerError;
                var message = "Unknown error occured.";
                var exceptionType = exception.GetType();
                switch(exception)
                {
                    case ServiceException e when exceptionType == typeof(ServiceException):
                        statusCode = HttpStatusCode.BadRequest;
                        message = exception.Message;
                        break;
                }
                context.Response.StatusCode = (int) statusCode;
                await context.Response.WriteAsJsonAsync(new { error = message });
            }));

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
