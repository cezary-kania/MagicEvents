using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MagicEvents.Api.Service.Api;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.RegisterUser;
using MagicEvents.Api.Service.Domain.Repositories;
using MagicEvents.Api.Service.IntrationTests.DataFactories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MagicEvents.Api.Service.IntrationTests.ControllersTests
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
                    builder.UseContentRoot(".");
                });
            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }

        public void Dispose()
        {
            var serviceScope = _serviceProvider.CreateScope();
            DeleteAllEvents(serviceScope);
        }

        private static void DeleteAllEvents(IServiceScope serviceScope)
        {
            var repository = serviceScope.ServiceProvider.GetService<IEventRepository>();
            var events = Task.Run(repository.GetAllAsync).Result;
            foreach (var @event in events)
            {
                Task.Run(async () => await repository.DeleteAsync(@event.Id));
            }
        }

        protected async Task AuthenticateAsync(RegisterUserDto registerUserDto)
        {
            var token = await GetUserJWT(registerUserDto);
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }   

        protected async Task AuthenticateAsync()
        {
            var registerUserDto = UserTestDataFactory.CreateTestUser();
            await AuthenticateAsync(registerUserDto);
        }

        protected void ClearAuthHeader()
        {
            TestClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task<string> GetUserJWT(RegisterUserDto registerUserDto)
        {
            var serializedUser = JsonConvert.SerializeObject(registerUserDto);
            var content = new StringContent(serializedUser, Encoding.UTF8, "application/json");
            var response = await TestClient.PostAsync("/Identity/register",content);
            var responseString = await response.Content.ReadAsStringAsync();
            var authTokenDto = JsonConvert.DeserializeObject<AuthTokenDto>(responseString);
            return authTokenDto.Token;
        }           
    }
}