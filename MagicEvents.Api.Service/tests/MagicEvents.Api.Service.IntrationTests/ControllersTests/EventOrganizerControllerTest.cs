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
            Guid userId = await GetUserId();
            var newCoorganizerDto = new AddCoOrganizerDto
            {
                UserId = userId
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            var eventId = Guid.NewGuid();
            await AuthenticateAsync();
            // Act  
            var response = await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers", content);
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
            string eventId = await CreateEvent();

            var newCoorganizerDto = new AddCoOrganizerDto
            {
                UserId = Guid.NewGuid()
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers", content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddCoOrganizer_WhenUserIsNotOrganizer_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            Guid userId = await GetUserId();

            await AuthenticateAsync();
            string eventId = await CreateEvent();

            var newCoorganizerDto = new AddCoOrganizerDto 
            {
                UserId = userId
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            await AuthenticateAsync();
            // Act  
            var response = await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddCoOrganizer_WhenUserIsOrganizer_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            Guid userId = await GetUserId();
            string eventId = await CreateEvent();

            var newCoorganizerDto = new AddCoOrganizerDto 
            {
                UserId = userId
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            // Act  
            var response = await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddCoOrganizer_WhenUserIsAlreadyCoOrganizer_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            Guid userId = await GetUserId();
            await AuthenticateAsync();
            string eventId = await CreateEvent();

            var newCoorganizerDto = new AddCoOrganizerDto 
            {
                UserId = userId
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            // Add coorganizer for a first time
            await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers",content);
            // Act  
            var response = await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers",content);
            // Arrange
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddCoOrganizer_WhenUserIsStandardParticipant_ShouldChangeRoleToCoOrganizer()
        {
            // Arrange
            RegisterUserDto registerOrganizerDto = new RegisterUserDto
            {
                Email = "organizer@example.com",
                Password = "P4$$w0rd1234"
            };
            await AuthenticateAsync(registerOrganizerDto);
            string eventId = await CreateEvent();
            RegisterUserDto registerParticipantDto = new RegisterUserDto
            {
                Email = "participant@example.com",
                Password = "P4$$w0rd1234"
            };
            await AuthenticateAsync(registerParticipantDto);
            await TestClient.PostAsync($"UserActivity/{eventId}", null);
            Guid userId = await GetUserId();
            LoginUserDto loginOrganizerDto = new LoginUserDto
            {
                Email = registerOrganizerDto.Email,
                Password = registerOrganizerDto.Password
            };
            await AuthenticateAsync(loginOrganizerDto);
            var newCoorganizerDto = new AddCoOrganizerDto 
            {
                UserId = userId
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            // Act  
            await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers",content);
            // Arrange
            LoginUserDto loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };
            await AuthenticateAsync(loginParticipantDto);
            var response = await TestClient.GetAsync("UserActivity");
            var responseString = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseString);
            var activity = activities.SingleOrDefault(x => x.EventId.ToString() == eventId);
            activity.Role
                .Should()
                .BeEquivalentTo("Co-Organizer");
        }

        [Fact]
        public async Task AddCoOrganizer_WhenUserIsNotRegisteredOnEvent_ShouldAddAsCoOrganizer()
        {
            // Arrange
            RegisterUserDto registerOrganizerDto = new RegisterUserDto
            {
                Email = "organizer@example.com",
                Password = "P4$$w0rd1234"
            };
            await AuthenticateAsync(registerOrganizerDto);
            string eventId = await CreateEvent();
            RegisterUserDto registerParticipantDto = new RegisterUserDto
            {
                Email = "participant@example.com",
                Password = "P4$$w0rd1234"
            };
            await AuthenticateAsync(registerParticipantDto);
            Guid userId = await GetUserId();
            LoginUserDto loginOrganizerDto = new LoginUserDto
            {
                Email = registerOrganizerDto.Email,
                Password = registerOrganizerDto.Password
            };
            await AuthenticateAsync(loginOrganizerDto);
            var newCoorganizerDto = new AddCoOrganizerDto 
            {
                UserId = userId
            };
            var serializedDto = JsonConvert.SerializeObject(newCoorganizerDto);
            var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");
            // Act  
            await TestClient.PostAsync($"EventOrganizer/{eventId}/coorganizers",content);
            // Arrange
            LoginUserDto loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };
            await AuthenticateAsync(loginParticipantDto);
            var response = await TestClient.GetAsync("UserActivity");
            var responseString = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseString);
            var activity = activities.SingleOrDefault(x => x.EventId.ToString() == eventId);
            activity.Role
                .Should()
                .BeEquivalentTo("Co-Organizer");
        }

        [Fact]
        public async Task RemoveParticipant_WhenUserIsNotInCrew_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            string eventId = await CreateEvent();
            await AuthenticateAsync();
            await TestClient.PostAsync($"UserActivity/{eventId}",null);
            Guid participantId = await GetUserId();
            await AuthenticateAsync();
            // Act
            var response = await TestClient.DeleteAsync($"EventOrganizer/{eventId}/participants/{participantId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveParticipant_WhenUserIsOrganizator_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            string eventId = await CreateEvent();
            Guid participantId = await GetUserId();
            // Act
            var response = await TestClient.DeleteAsync($"EventOrganizer/{eventId}/participants/{participantId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveParticipant_WhenUserWasNotRegistered_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            Guid participantId = await GetUserId();
            await AuthenticateAsync();
            string eventId = await CreateEvent();
            // Act
            var response = await TestClient.DeleteAsync($"EventOrganizer/{eventId}/participants/{participantId}");
            // Assert 
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveParticipant_WhenUserWasRegistered_ShouldRemoveActivity()
        {
            // Arrange
            var registerOrganizerDto = new RegisterUserDto
            {
                Email = "organizer@example.com",
                Password = "P4$$w0rd1234"
            };
            await AuthenticateAsync(registerOrganizerDto);
            string eventId = await CreateEvent();
            var registerParticipantDto = new RegisterUserDto
            {
                Email = "participant@example.com",
                Password = "P4$$w0rd1234"
            };
            await AuthenticateAsync(registerParticipantDto);
            await TestClient.PostAsync($"UserActivity/{eventId}", null);
            Guid userId = await GetUserId();
            var loginOrganizerDto = new LoginUserDto
            {
                Email = registerOrganizerDto.Email,
                Password = registerOrganizerDto.Password
            };
            await AuthenticateAsync(loginOrganizerDto);
            // Act
            var response = await TestClient.DeleteAsync($"EventOrganizer/{eventId}/participants/{userId}");
            // Assert 
            var loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };
            await AuthenticateAsync(loginParticipantDto);
            response = await TestClient.GetAsync("UserActivity");
            var responseBody = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseBody);
            var activity = activities.SingleOrDefault(x => x.EventId.ToString() == eventId);
            activity.Should()
                .BeNull();
        }
        
        [Fact]
        public async Task BanParticipant_WhenUserIdIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            var userId = Guid.NewGuid();
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/participants/{userId}", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task BanParticipant_WhenEventIdIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserId();
            await AuthenticateAsync();
            var eventId = Guid.NewGuid();
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/participants/{userId}", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task BanParticipant_WhenUserIsNotInCrew_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            await AuthenticateAsync();
            var userId = await GetUserId();
            await TestClient.PostAsync($"UserActivity/{eventId}",null);
            await AuthenticateAsync();
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/participants/{userId}", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task BanParticipant_WhenBannedUserIsOranizer_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            var userId = await GetUserId();
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/participants/{userId}", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task BanParticipant_WhenUserIsNotRegistered_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterUserDto 
            {
                Email = "organizer@example.com",
                Password = "P4$$w0rd1234"
            };
            await AuthenticateAsync(registerDto);
            var eventId = await CreateEvent();
            await AuthenticateAsync();
            var userId = await GetUserId();
            var loginDto = new LoginUserDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            };
            await AuthenticateAsync(loginDto);
            // Act
            var response = await TestClient.PatchAsync($"EventOrganizer/{eventId}/participants/{userId}", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task BanParticipant_WhenParamsAreValid_ShouldBanUser()
        {
            // Arrange
            var registerOrganizerDto = new RegisterUserDto 
            {
                Email = "organizer@example.com",
                Password = "P4$$w0rd1234"
            };
            var registerParticipantDto = new RegisterUserDto 
            {
                Email = "participant@example.com",
                Password = "P4$$w0rd1234"
            };
            await AuthenticateAsync(registerOrganizerDto);
            var eventId = await CreateEvent();
            await AuthenticateAsync(registerParticipantDto);
            var userId = await GetUserId();
            await TestClient.PostAsync($"UserActivity/{eventId}",null);
            var loginOrganizerDto = new LoginUserDto
            {
                Email = registerOrganizerDto.Email,
                Password = registerOrganizerDto.Password
            };
            await AuthenticateAsync(loginOrganizerDto);
            // Act
            await TestClient.PatchAsync($"EventOrganizer/{eventId}/participants/{userId}", null);
            // Assert
            var loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };
            await AuthenticateAsync(loginParticipantDto);
            var response = await TestClient.GetAsync("UserActivity");
            var responseBody = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseBody);
            var activity = activities.SingleOrDefault(x => x.EventId.ToString() == eventId);
            activity.Status
                .Should()
                .BeEquivalentTo("Banned");
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
            var response = await TestClient.GetAsync($"User/userData");
            var responseBodyString = await response.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserDto>(responseBodyString);
            userData.EventActivities
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

        private async Task<Guid> GetUserId()
        {   
            var userResponse = await TestClient.GetAsync("User/userData");
            var userResponseString = await userResponse.Content.ReadAsStringAsync();
            var userId = JsonConvert.DeserializeObject<UserDto>(userResponseString).Id;
            return userId;
        }
    }
}