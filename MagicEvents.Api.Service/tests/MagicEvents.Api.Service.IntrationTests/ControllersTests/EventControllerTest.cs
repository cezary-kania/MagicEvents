using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Events.CreateEvent;
using MagicEvents.Api.Service.Domain.Entities;
using MagicEvents.Api.Service.IntrationTests.DataFactories;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntrationTests.ControllerTests
{
    public class EventControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAllEvents_WhenEventListEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            // Act
            var response = await TestClient.GetAsync("/Event");
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            JsonConvert.DeserializeObject<IEnumerable<Event>>(responseString)
                .Should()
                .BeEmpty();
        }

        [Fact]
        public async Task GetAllEvents_WhenEventListNotEmpty_ShouldReturnNotEmptyEventList()
        {
            // Arrange
            await AuthenticateAsync();
            var createEventDto = EventTestDataFactory.CreateTestEventDto();
            var serializedEvent = JsonConvert.SerializeObject(createEventDto);
            var content = new StringContent(serializedEvent, Encoding.UTF8, "application/json");
            await TestClient.PostAsync("/EventOrganizer", content);
            // Act
            var response = await TestClient.GetAsync("/Event");
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            JsonConvert.DeserializeObject<IEnumerable<Event>>(responseString)
                .Should()
                .NotBeEmpty();
        }

        [Fact]
        public async Task GetEvent_WhenEventIdIsValid_ShouldReturnEventData()
        {
            // Arrange
            await AuthenticateAsync();
            var createEventDto = EventTestDataFactory.CreateTestEventDto();
            var serializedEvent = JsonConvert.SerializeObject(createEventDto);
            var content = new StringContent(serializedEvent, Encoding.UTF8, "application/json");
            var createdEventResponse = await TestClient.PostAsync("/EventOrganizer", content);
            createdEventResponse.Headers.TryGetValues("location", out var location);
            string eventLocation = location.ToList()[0];
            var eventId = eventLocation.Split('/').Last();
            // Act
            var response = await TestClient.GetAsync($"/Event/{eventId}");
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            JsonConvert.DeserializeObject<Event>(responseString)
                .Should()
                .NotBeNull();
        }

        [Fact]
        public async Task GetEvent_WhenEventIdIsNotValid_ShouldReturnNotFound()
        {
            // Arrange
            var eventId = Guid.Empty;
            // Act
            var response = await TestClient.GetAsync($"/Event/{eventId}");
            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetThumbnail_WhenEventIdIsNotValid_ShouldReturnNotFound()
        {
            // Arrange
            var eventId = Guid.Empty;
            // Act
            var response = await TestClient.GetAsync($"/Event/{eventId}/thumbnail");
            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetThumbnail_WhenEventIdIsValidButThumbnailIsNotSet_ShouldReturnNotFound()
        {
            // Arrange
            await AuthenticateAsync();
            var createEventDto = EventTestDataFactory.CreateTestEventDto();
            var serializedEvent = JsonConvert.SerializeObject(createEventDto);
            var content = new StringContent(serializedEvent, Encoding.UTF8, "application/json");
            var createdEventResponse = await TestClient.PostAsync("/EventOrganizer", content);
            createdEventResponse.Headers.TryGetValues("location", out var location);
            string eventLocation = location.ToList()[0];
            var eventId = eventLocation.Split('/').Last();
            // Act
            var response = await TestClient.GetAsync($"/Event/{eventId}/thumbnail");
            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.NotFound);
        }
    }
}