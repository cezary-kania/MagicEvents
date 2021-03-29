using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Domain.Entities;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntrationTests
{
    public class EventControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetAllEvents_WhenEventListEmpty_ReturnsEmptyList()
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
    }
}