using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Events;
using MagicEvents.Api.Service.Application.DTOs.Events.AddCoOrganizer;
using MagicEvents.Api.Service.Application.DTOs.Events.UpdateEvent;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.LoginUser;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.RegisterUser;
using MagicEvents.Api.Service.IntegrationTests.DataFactories;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntegrationTests.ControllersTests
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
        public async Task DeleteEvent_WhenInvalidEventId_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = Guid.NewGuid();
            // Act
            var response = await TestClient.DeleteAsync($"EventOrganizer/{eventId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteEvent_WhenUserIsNotOrganizer_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = CreateEvent();

            await AuthenticateAsync();
            // Act
            var response = await TestClient.DeleteAsync($"EventOrganizer/{eventId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteEvent_WhenEventWasCreated_ShouldDeleteEventFromList()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            // Act
            await TestClient.DeleteAsync($"EventOrganizer/{eventId}");
            // Assert
            var response = await TestClient.GetAsync($"Event/{eventId}");
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteEvent_WhenSomeUserWasRegistered_ShouldDeleteEventFromActivities()
        {
            // Arrange
            var registerOrganizerDto = new RegisterUserDto 
            {
                Email = "organizer@example.com",
                Password = "P4$$w0rd1234"
            };
            var loginOrganizerDto = new LoginUserDto
            {
                Email = registerOrganizerDto.Email,
                Password = registerOrganizerDto.Password
            };
            var registerParticipantDto = new RegisterUserDto 
            {
                Email = "participant@example.com",
                Password = "P4$$w0rd1234"
            };
            var loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };

            await AuthenticateAsync(registerOrganizerDto);
            var eventId = await CreateEvent();
            await AuthenticateAsync(registerParticipantDto);
            await TestClient.PostAsync($"Event/{eventId}",null);
            await AuthenticateAsync(loginOrganizerDto);
            // Act
            await TestClient.DeleteAsync($"EventOrganizer/{eventId}");
            // Assert
            await AuthenticateAsync(loginParticipantDto);
            var userId = await GetUserId();
            var response = await TestClient.GetAsync($"UserActivity/{userId}");
            var responseBodyString = await response.Content.ReadAsStringAsync();
            var userActivities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseBodyString);
            userActivities
                .SingleOrDefault(x => x.EventId.ToString() == eventId)
                .Should()
                .BeNull();  
        }

        [Fact]
        public async Task CancelEvent_WhenEventIdIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = Guid.NewGuid();
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/cancel", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CancelEvent_WhenUserNotInCrew_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            await AuthenticateAsync();
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/cancel", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CancelEvent_WhenParamsAreValid_ShouldCancelEvent()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            // Act
            await TestClient.PatchAsync($"EventOrganizer/{eventId}/cancel", null);
            // Assert
            var response = await TestClient.GetAsync($"Event/{eventId}");
            var responseString = await response.Content.ReadAsStringAsync();
            var @event = JsonConvert.DeserializeObject<EventDto>(responseString);
            @event.Status
                .Should()
                .BeEquivalentTo("Canceled");
        }

        [Fact]
        public async Task SetThumbnail_WhenEventIdIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = Guid.NewGuid();
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/thumbnail", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SetThumbnail_WhenUserNotInCrew_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            await AuthenticateAsync();
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/thumbnail", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateEvent_WhenEventIdIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = Guid.NewGuid();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            var contentString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PutAsync($"EventOrganizer/{eventId}", content);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateEvent_WhenUserNotInCrew_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            var contentString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            await AuthenticateAsync();
            // Act
            var response = await TestClient.PutAsync($"EventOrganizer/{eventId}", content);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task UpdateEvent_WhenParamsAreNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            newEventDto.StartsAt = DateTime.UtcNow.AddDays(-15);
            var contentString = JsonConvert.SerializeObject(newEventDto);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PutAsync($"EventOrganizer/{eventId}", content);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateEvent_WhenParamsAreValid_ShouldUpdateEvent()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            var updateEventDto = EventTestDataFactory.CreateTestEventDto();
            var contentString = JsonConvert.SerializeObject(updateEventDto);
            var content = new StringContent(contentString, Encoding.UTF8, "application/json");
            // Act
            await TestClient.PutAsync($"EventOrganizer/{eventId}", content);
            // Assert
            var response = await TestClient.GetAsync($"Event/{eventId}");
            var responseBodyString = await response.Content.ReadAsStringAsync();
            var @event = JsonConvert.DeserializeObject<EventDto>(responseBodyString);
            @event.Title
                .Should()
                .BeEquivalentTo(updateEventDto.Title);
        }

        private async Task<string> CreateEvent()
        {
            var newEventDto = EventTestDataFactory.CreateTestEventDto();
            var newEventDtoString = JsonConvert.SerializeObject(newEventDto);
            var newEventContent = new StringContent(newEventDtoString, Encoding.UTF8, "application/json");
            var newEventResponse = await TestClient.PostAsync("EventOrganizer", newEventContent);
            newEventResponse.Headers.TryGetValues("location", out var locations);
            var eventId = locations.ToList()[0].Split('/').Last();
            return eventId;
        }
    }
}