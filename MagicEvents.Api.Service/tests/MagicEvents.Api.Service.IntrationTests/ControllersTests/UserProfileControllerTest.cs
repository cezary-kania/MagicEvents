using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntrationTests.ControllersTests
{
    public class UserProfileControllerTest : IntegrationTest
    {
        [Fact]
        public async Task GetProfile_WhenUserIsAuthenticated_ShouldReturnOKStatus()
        {
            // Arrange
            await AuthenticateAsync();
            var userInfoResponse = await TestClient.GetAsync("User/UserData");
            var userInfoResponseString = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<UserDto>(userInfoResponseString); 
            // Act
            var response = await TestClient.GetAsync($"UserProfile/{userInfo.Id}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetProfile_WhenUserIsNotAuthenticated_ShouldReturnOKStatus()
        {
            // Arrange
            await AuthenticateAsync();
            var userInfoResponse = await TestClient.GetAsync("User/UserData");
            var userInfoResponseString = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<UserDto>(userInfoResponseString);
            ClearAuthHeader(); 
            // Act
            var response = await TestClient.GetAsync($"UserProfile/{userInfo.Id}");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetProfile_WhenUserIsAuthenticated_ShouldReturnValidProfile()
        {
            // Arrange
            await AuthenticateAsync();
            var updatedProfileDto = new UpdateProfileDto
            {
                FirstName = "Test firstname",
                LastName = "Test lastName",
                Informations = "Test informations"
            };
            await UpdateUserProfileAsync(updatedProfileDto);
            Guid userId = await GetUserIdAsync();
            // Act
            var response = await TestClient.GetAsync($"UserProfile/{userId}");
            var responseString = await response.Content.ReadAsStringAsync();
            var profile = JsonConvert.DeserializeObject<UserProfileBaseDto>(responseString);
            // Assert
            profile.FirstName
                .Should()
                .BeEquivalentTo(updatedProfileDto.FirstName);
            profile.LastName
                .Should()
                .BeEquivalentTo(updatedProfileDto.LastName);
        }

        [Fact]
        public async Task UpdateProfile_WhenUserIsNotAuthenticated_ShouldReturnUnauthorizedStatus()
        {
            // Arrange
            var updatedProfileDto = new UpdateProfileDto
            {
                FirstName = "Test firstname",
                LastName = "Test lastName",
                Informations = "Test informations"
            };
            var profileDtoString = JsonConvert.SerializeObject(updatedProfileDto);
            var content = new StringContent(profileDtoString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PutAsync("UserProfile", content);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateProfile_WhenUserIsAuthenticated_ShouldReturnNoContentStatus()
        {
            // Arrange
            await AuthenticateAsync();
            var updatedProfileDto = new UpdateProfileDto
            {
                FirstName = "Test firstname",
                LastName = "Test lastName",
                Informations = "Test informations"
            };
            var profileDtoString = JsonConvert.SerializeObject(updatedProfileDto);
            var content = new StringContent(profileDtoString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PutAsync("UserProfile", content);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.NoContent);
        }
        
        [Fact]
        public async Task UpdateProfile_WhenUserIsAuthenticated_ShouldChangeProfileData()
        {
            // Arrange
            await AuthenticateAsync();
            
            var initialUpdateProfileDto = new UpdateProfileDto
            {
                FirstName = "Test firstname",
                LastName = "Test lastName",
                Informations = "Test informations"
            };
            await UpdateUserProfileAsync(initialUpdateProfileDto);
            var updatedProfileDto = new UpdateProfileDto
            {
                FirstName = "Updated firstname",
                LastName = "Updated lastName",
                Informations = "Updated informations"
            };
            var profileDtoString = JsonConvert.SerializeObject(updatedProfileDto);
            var content = new StringContent(profileDtoString, Encoding.UTF8, "application/json");
            Guid userId = await GetUserIdAsync();

            // Act
            await TestClient.PutAsync("UserProfile", content);
            var response = await TestClient.GetAsync($"UserProfile/{userId}");
            var responseString = await response.Content.ReadAsStringAsync();
            var profile = JsonConvert.DeserializeObject<UserProfileBaseDto>(responseString);
            
            // Assert
            profile.FirstName
                .Should()
                .NotBeNullOrWhiteSpace()
                .And.NotBeEquivalentTo(initialUpdateProfileDto.FirstName)
                .And.BeEquivalentTo(updatedProfileDto.FirstName);

            profile.LastName
                .Should()
                .NotBeNullOrWhiteSpace()
                .And.NotBeEquivalentTo(initialUpdateProfileDto.LastName)
                .And.BeEquivalentTo(updatedProfileDto.LastName);
        }

        private async Task<Guid> GetUserIdAsync()
        {
            var userInfoResponse = await TestClient.GetAsync("User/UserData");
            var userInfoResponseString = await userInfoResponse.Content.ReadAsStringAsync();
            var userId = JsonConvert.DeserializeObject<UserDto>(userInfoResponseString).Id;
            return userId;
        }

        private async Task UpdateUserProfileAsync(UpdateProfileDto updatedProfileDto)
        {
            var profileDtoString = JsonConvert.SerializeObject(updatedProfileDto);
            var content = new StringContent(profileDtoString, Encoding.UTF8, "application/json");
            await TestClient.PutAsync("UserProfile", content);
        }

    }
}