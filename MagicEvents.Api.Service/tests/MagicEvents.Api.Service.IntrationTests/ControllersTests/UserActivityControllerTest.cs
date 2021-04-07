using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.IntrationTests.DataFactories;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntrationTests.ControllersTests
{
    public class UserActivityControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetActivities_WhenUserIsUnauthorized_ShouldReturnUnauthorized()
        {
            // Arrange
            
            // Act
            var response = await TestClient.GetAsync("UserActivity");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetActivities_WhenUserIsNotRegisteredForAnyEvent_ShouldReturnEmptyList()
        {
            // Arrange
            await AuthenticateAsync();
            // Act
            var response = await TestClient.GetAsync("UserActivity");
            var responseString = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseString);
            // Assert
            activities.Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetActivities_WhenUserIsRegisteredForSomeEvents_ShouldReturnNotEmptyList()
        {
            // Arrange
            string eventId = await CreateRandomNewEvent();
            await AuthenticateAsync();
            await TestClient.PostAsync($"UserActivity/{eventId}", null);
            // Act
            var response = await TestClient.GetAsync("UserActivity");
            var responseString = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseString);
            // Assert
            activities.Should()
                .NotBeEmpty();
        }

        [Fact]
        public async Task GetActivities_WhenUserIsRegisteredForTwoEvents_ShouldReturnTwoItemsList()
        {
            // Arrange
            int eventsNum = 2;
            List<string> eventIds = new List<string>();
            for(int i = 0; i < eventsNum; ++i)
            {
                eventIds.Add(await CreateRandomNewEvent());
            } 
            await AuthenticateAsync();
            foreach(string eventId in eventIds)
            {
                await TestClient.PostAsync($"UserActivity/{eventId}", null);
            }
            // Act
            var response = await TestClient.GetAsync("UserActivity");
            var responseString = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseString);
            // Assert
            activities.ToList()
                .Count
                .Should()
                .Be(eventsNum);
        }

        [Fact]
        public async Task RegisterOnEvent_WhenNotRegisteredYet_ShouldReturnNoContent()
        {
            // Arrange
            string eventId = await CreateRandomNewEvent();
            await AuthenticateAsync();
            // Act
            var response =await TestClient.PostAsync($"UserActivity/{eventId}", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task RegisterOnEvent_WhenNotRegisteredYet_ShouldRegisterOnEvent()
        {
            // Arrange
            string eventId = await CreateRandomNewEvent();
            await AuthenticateAsync();
            // Act
            await TestClient.PostAsync($"UserActivity/{eventId}", null);
            // Assert
            var response = await TestClient.GetAsync("UserActivity");
            var responseString = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseString);

            activities.SingleOrDefault(x => x.EventId.ToString() == eventId)
                .Should()
                .NotBeNull();
        }

        [Fact]
        public async Task RegisterOnEvent_WhenIsRegisteredAlready_ShouldReturnBadRequest()
        {
            // Arrange
            string eventId = await CreateRandomNewEvent();
            await AuthenticateAsync();
            await TestClient.PostAsync($"UserActivity/{eventId}", null);
            // Act
            var response = await TestClient.PostAsync($"UserActivity/{eventId}", null);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task LeaveEvent_WhenUserIsRegisteredAndItIsNotOrganizer_ShouldReturnNoContent()
        {
            // Arrange
            string eventId = await CreateRandomNewEvent();
            await AuthenticateAsync();
            await TestClient.PostAsync($"UserActivity/{eventId}", null);
            // Act
            var response = await TestClient.DeleteAsync($"UserActivity/{eventId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task LeaveEvent_WhenUserIsRegisteredAndItIsNotOrganizer_ShouldMarkActivityAsLeft()
        {
            // Arrange
            string eventId = await CreateRandomNewEvent();
            await AuthenticateAsync();
            await TestClient.PostAsync($"UserActivity/{eventId}", null);
            // Act
            await TestClient.DeleteAsync($"UserActivity/{eventId}");
            // Assert
            var response = await TestClient.GetAsync("UserActivity");
            var responseString = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<IEnumerable<UserEventActivityDto>>(responseString);

            var activity = activities.SingleOrDefault(x => x.EventId == Guid.Parse(eventId));
            activity.Status
                .Should()
                .NotBeEmpty()
                .And
                .BeEquivalentTo("Left");
        }

        [Fact]
        public async Task LeaveEvent_WhenUserIsNotOrganizer_ShouldReturnBadRequest()
        {
            // Arrange
            string eventId = await CreateRandomNewEvent();
            // Act
            var response = await TestClient.DeleteAsync($"UserActivity/{eventId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task LeaveEvent_WhenEventNotExist_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var eventId = Guid.NewGuid();
            // Act
            var response = await TestClient.DeleteAsync($"UserActivity/{eventId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }
        
        [Fact]
        public async Task LeaveEvent_WhenIsNotRegisteredForEvent_ShouldReturnBadRequest()
        {
            // Arrange
            string eventId = await CreateRandomNewEvent();
            await AuthenticateAsync();
            // Act
            var response = await TestClient.DeleteAsync($"UserActivity/{eventId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        private async Task<string> CreateRandomNewEvent()
        {
            await AuthenticateAsync();
            var newEvent = EventTestDataFactory.CreateTestEventDto();
            var newEventString = JsonConvert.SerializeObject(newEvent);
            var content = new StringContent(newEventString, Encoding.UTF8, "application/json");
            var createEventResponse = await TestClient.PostAsync("EventOrganizer", content);
            createEventResponse.Headers.TryGetValues("location", out var location);
            var eventId = location.ToList()[0].Split('/').Last();
            return eventId;
        }
    }
}