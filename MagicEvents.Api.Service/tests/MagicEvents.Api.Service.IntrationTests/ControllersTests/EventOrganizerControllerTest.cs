using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Events.AddCoOrganizer;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.IntrationTests.DataFactories;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntrationTests.ControllersTests
{
    public class EventOrganizerControllerTest : IntegrationTest
    {
        [Fact]
        public async Task CreateEvent_WhenUserIsUnauthorized_ShouldReturnUnauthorized()
        {
            // Arrange
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync("EventOrganizer",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateEvent_WhenStartDateIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            newEventDto.StartsAt = DateTime.UtcNow.AddDays(-2);
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync("EventOrganizer",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateEvent_WhenDatesConfigurationIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            newEventDto.StartsAt = DateTime.UtcNow.AddDays(5);
            newEventDto.EndsAt = DateTime.UtcNow.AddDays(3);
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync("EventOrganizer",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateEvent_WhenTitleIsBlank_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            newEventDto.Title = string.Empty;
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync("EventOrganizer",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateEvent_WhenTitleIsToLong_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            newEventDto.Title = new string('A',101);
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync("EventOrganizer",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateEvent_WhenDescriptionIsToLong_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            newEventDto.Description = new string('A',4001);
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync("EventOrganizer",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateEvent_WhenEventDtoIsValid_ShouldReturnCreated()
        {
            // Arrange
            await AuthenticateAsync();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync("EventOrganizer",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.Created);
        }

        [Fact]
        public async Task CreateEvent_WhenEventDtoIsValid_ShouldAddNewEventToList()
        {
            // Arrange
            await AuthenticateAsync();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync("EventOrganizer",content);
            // Arrange
            response.Headers.TryGetValues("location", out var locations);
            var eventId = locations.ToList()[0].Split('/').Last();
            response = await TestClient.GetAsync($"Event/{eventId}");
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AddCoOrganizer_WhenEventIdIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var userResponse = await TestClient.GetAsync("User/userData");
            var userResponseString = await userResponse.Content.ReadAsStringAsync();
            var newCoorganizerDto = new AddCoOrganizerDto 
            {
                UserId = JsonConvert.DeserializeObject<UserDto>(userResponseString).Id
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            var eventId = Guid.NewGuid();
            await AuthenticateAsync();
            // Act  
            var response = await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddCoOrganizer_WhenCoOrganizerIdIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(newEventDtoString, Encoding.UTF8, "application/json"); 
            var response = await TestClient.PostAsync("EventOrganizer",content);
            response.Headers.TryGetValues("location", out var locations);
            var eventId = locations.ToList()[0].Split('/').Last();

            var newCoorganizerDto = new AddCoOrganizerDto 
            {
                UserId = Guid.NewGuid()
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            // Act  
            response = await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        
    }
}