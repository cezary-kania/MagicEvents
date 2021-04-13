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
    public class EventParticipantsOrganizerControllerTest : IntegrationTest
    {
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
            var response = await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers", content);
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
            var response = await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers", content);
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
            var response = await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers",content);
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
            var response = await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers",content);
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
            await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers",content);
            // Act  
            var response = await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers",content);
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
            await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers",content);
            // Arrange
            LoginUserDto loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };
            await AuthenticateAsync(loginParticipantDto);
            userId = await GetUserId();
            var response = await TestClient.GetAsync($"UserActivity/{userId}");
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
            await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers",content);
            // Arrange
            LoginUserDto loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };
            await AuthenticateAsync(loginParticipantDto);
            var response = await TestClient.GetAsync($"UserActivity/{userId}");
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
            var response = await TestClient.DeleteAsync($"EventParticipantsOrganizer/{eventId}/participants/{participantId}");
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
            var response = await TestClient.DeleteAsync($"EventParticipantsOrganizer/{eventId}/participants/{participantId}");
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
            var response = await TestClient.DeleteAsync($"EventParticipantsOrganizer/{eventId}/participants/{participantId}");
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
            var response = await TestClient.DeleteAsync($"EventParticipantsOrganizer/{eventId}/participants/{userId}");
            // Assert 
            var loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };
            await AuthenticateAsync(loginParticipantDto);
            response = await TestClient.GetAsync($"UserActivity/{userId}");
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
            var response = await TestClient.PatchAsync($"EventParticipantsOrganizer/{eventId}/participants/{userId}", null);
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
            var response = await TestClient.PatchAsync($"EventParticipantsOrganizer/{eventId}/participants/{userId}", null);
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
            var response = await TestClient.PatchAsync($"EventParticipantsOrganizer/{eventId}/participants/{userId}", null);
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
            var response = await TestClient.PatchAsync($"EventParticipantsOrganizer/{eventId}/participants/{userId}", null);
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
            var response = await TestClient.PatchAsync($"EventParticipantsOrganizer/{eventId}/participants/{userId}", null);
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
            await TestClient.PatchAsync($"EventParticipantsOrganizer/{eventId}/participants/{userId}", null);
            // Assert
            var loginParticipantDto = new LoginUserDto
            {
                Email = registerParticipantDto.Email,
                Password = registerParticipantDto.Password
            };
            await AuthenticateAsync(loginParticipantDto);
            var response = await TestClient.GetAsync($"UserActivity/{userId}");
            var responseBody = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseBody);
            var activity = activities.SingleOrDefault(x => x.EventId.ToString() == eventId);
            activity.Status
                .Should()
                .BeEquivalentTo("Banned");
        }

        [Fact]
        public async Task RemoveCoOrganizer_WhenInvalidCoorganizerId_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = CreateEvent();
            var coOrganizerId = Guid.NewGuid();
            // Act 
            var response = await TestClient
                .DeleteAsync($"EventParticipantsOrganizer/{eventId}/coorganizers/{coOrganizerId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveCoOrganizer_WhenEventIdIsInvalid_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var coOrganizerId = await GetUserId();
            await AuthenticateAsync();
            var eventId = Guid.NewGuid();
            // Act 
            var response = await TestClient
                .DeleteAsync($"EventParticipantsOrganizer/{eventId}/coorganizers/{coOrganizerId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveCoOrganizer_WhenEventIdIsNotOrganizer_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var coOrganizerId = await GetUserId();

            await AuthenticateAsync();
            var eventId = await CreateEvent();
            var addCoOrganizerDto = new AddCoOrganizerDto
            {
                UserId = coOrganizerId
            };
            var payload = JsonConvert.SerializeObject(addCoOrganizerDto);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers", content);
            await AuthenticateAsync();
            // Act 
            var response = await TestClient
                .DeleteAsync($"EventParticipantsOrganizer/{eventId}/coorganizers/{coOrganizerId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveCoOrganizer_WhenUserIsNotRegisteredOnEvent_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var coOrganizerId = await GetUserId();

            await AuthenticateAsync();
            var eventId = await CreateEvent();
            // Act 
            var response = await TestClient
                .DeleteAsync($"EventParticipantsOrganizer/{eventId}/coorganizers/{coOrganizerId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveCoOrganizer_WhenUserIsNotCoOrganizer_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Email = "organizer@example.com",
                Password = "P4$$w0rd1234"
            };
            var loginDto = new LoginUserDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            };
            await AuthenticateAsync(registerDto);
            var eventId = await CreateEvent();
            await AuthenticateAsync();
            var userId = await GetUserId();
            await TestClient.PostAsync($"UserActivity/{eventId}",null);
            await AuthenticateAsync(loginDto);
            // Act
            var response = await TestClient
                .DeleteAsync($"EventParticipantsOrganizer/{eventId}/coorganizers/{userId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveCoOrganizer_WhenUserIsCoOrganizer_ShouldChangeEventRole()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserId();
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            var payload = new AddCoOrganizerDto 
            {
                UserId = userId
            };
            var payloadString = JsonConvert.SerializeObject(payload);
            var content = new StringContent(payloadString, Encoding.UTF8, "application/json");
            await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers", content);
            // Act
            await TestClient.DeleteAsync($"EventParticipantsOrganizer/{eventId}/coorganizers/{userId}");
            // Assert
            var response = await TestClient.GetAsync($"UserActivity/{userId}");
            var responseBody = await response.Content.ReadAsStringAsync();
            var userActivities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseBody);
            var activity = userActivities.SingleOrDefault(x => x.EventId.ToString() == eventId);
            
            activity.Should().NotBeNull();
            activity?.Role
                .Should()
                .BeEquivalentTo("StandardParticipant");
        }

        [Fact]
        public async Task RemoveCoOrganizer_WhenUserIsCoOrganizer_ShouldAddUserToStandardParticipants()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserId();
            await AuthenticateAsync();
            var eventId = await CreateEvent();
            var payload = new AddCoOrganizerDto 
            {
                UserId = userId
            };
            var payloadString = JsonConvert.SerializeObject(payload);
            var content = new StringContent(payloadString, Encoding.UTF8, "application/json");
            await TestClient.PostAsync($"EventParticipantsOrganizer/{eventId}/coorganizers", content);
            // Act
            await TestClient.DeleteAsync($"EventParticipantsOrganizer/{eventId}/coorganizers/{userId}");
            // Assert
            var response = await TestClient.GetAsync($"Event/{eventId}");
            var responseBody = await response.Content.ReadAsStringAsync();
            var @event = JsonConvert.DeserializeObject<EventDto>(responseBody);
            @event.Participants
                .StandardParticipants
                .SingleOrDefault(x => x.ToString() == eventId)
                .Should()
                .NotBe(userId);
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