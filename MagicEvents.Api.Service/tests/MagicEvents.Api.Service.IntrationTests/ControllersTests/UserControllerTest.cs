using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.IntrationTests.DataFactories;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntrationTests.ControllersTests
{
    public class UserControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetUser_WhenUserIsAuthenticated_ShouldReturnUserInfo()
        {
            // Arrange
            var testUser = UserTestDataFactory.CreateTestUser();
            await AuthenticateAsync(testUser);
            // Act
            var response = await TestClient.GetAsync("User/userData");
            var contentString = await response.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<UserDto>(contentString); 
            // Assert
            content.Identity.Email
                .Should()
                .BeEquivalentTo(testUser.Email);
        }

        [Fact]
        public async Task GetUser_WhenUserIsAuthenticated_ShouldReturnOKStatus()
        {
            // Arrange
            var testUser = UserTestDataFactory.CreateTestUser();
            await AuthenticateAsync();
            // Act
            var response = await TestClient.GetAsync("User/userData");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetUser_WhenUserIsNotAuthenticated_ShouldReturnUnauthorizedStatus()
        {
            // Arrange
            // Act
            var response = await TestClient.GetAsync("User/userData");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.Unauthorized);
        }
    }
}