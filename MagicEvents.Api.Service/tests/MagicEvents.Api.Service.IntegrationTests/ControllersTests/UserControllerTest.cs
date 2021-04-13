using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.IntegrationTests.DataFactories;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntegrationTests.ControllersTests
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

        [Fact]
        public async Task GetUserById_WhenUserIsAuthenticated_ShouldReturnUserInfo()
        {
            // Arrange
            var testUser = UserTestDataFactory.CreateTestUser();
            await AuthenticateAsync(testUser);
            var userId = await GetUserId();
            // Act
            var response = await TestClient.GetAsync($"User/userData/{userId}");
            var contentString = await response.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<UserDto>(contentString); 
            // Assert
            content.Identity.Email
                .Should()
                .BeEquivalentTo(testUser.Email);
        }

        [Fact]
        public async Task GetUserById_WhenUserIsAuthenticated_ShouldReturnOKStatus()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserId();
            // Act
            var response = await TestClient.GetAsync($"User/userData/{userId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetUserById_WhenUserIsNotAuthenticated_ShouldReturnUnauthorizedStatus()
        {
            // Arrange
            var testUser = UserTestDataFactory.CreateTestUser();
            await AuthenticateAsync(testUser);
            var userId = await GetUserId();
            ClearAuthHeader();
            // Act
            var response = await TestClient.GetAsync($"User/userData/{userId}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.Unauthorized);
        }
    }
}