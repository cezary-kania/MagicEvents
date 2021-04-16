using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users;
using MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntegrationTests.ControllersTests
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

        [Fact]
        public async Task GetProfileImage_WhenUserIdIsNotValid_ShouldFail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            // Act
            var response = await TestClient.GetAsync($"UserProfile/{userId}/profileImage");
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetProfileImage_WhenUserIdIsValid_ShouldReturnImageFile()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserIdAsync();
            using var fileStream = File.OpenRead("SampleData/ValidImageFile.jpg");
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            var formData = new MultipartFormDataContent();
            formData.Add(streamContent, "file","ValidImageFile.jpg");
            await TestClient.PatchAsync($"UserProfile/profileImage", formData);
            ClearAuthHeader();
            // Act
            var response = await TestClient.GetAsync($"UserProfile/{userId}/profileImage");
            // Assert
            var responseContent = await response.Content.ReadAsByteArrayAsync();
            var result = new FileContentResult(responseContent, "image/jpeg");
            result.FileContents
                .Should()
                .NotBeEmpty();
        }

        [Fact]
        public async Task UpdateProfileImage_WhenUserNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserIdAsync();
            using var fileStream = File.OpenRead("SampleData/ValidImageFile.jpg");
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            var formData = new MultipartFormDataContent();
            formData.Add(streamContent, "file","ValidImageFile.jpg");
            ClearAuthHeader();
            // Act
            var response = await TestClient.PatchAsync($"UserProfile/profileImage", formData);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateProfileImage_WhenFileIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserIdAsync();
            var formData = new MultipartFormDataContent();
            // Act
            var response = await TestClient.PatchAsync($"UserProfile/profileImage", formData);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateProfileImage_WhenFileIsNotImage_ShouldReturnBadRequest()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserIdAsync();
            using var fileStream = File.OpenRead("SampleData/NotImageFile.txt");
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            var formData = new MultipartFormDataContent();
            formData.Add(streamContent, "file","NotImageFile.txt");
            // Act
            var response = await TestClient.PatchAsync($"UserProfile/profileImage", formData);
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateProfileImage_WhenImageIsValid_ShouldSetNewProfileImage()
        {
            // Arrange
            await AuthenticateAsync();
            var userId = await GetUserIdAsync();
            using var fileStream = File.OpenRead("SampleData/ValidImageFile.jpg");
            var streamContent = new StreamContent(fileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            var formData = new MultipartFormDataContent();
            formData.Add(streamContent, "file","ValidImageFile.jpg");
            // Act
            await TestClient.PatchAsync($"UserProfile/profileImage", formData);
            // Assert
            await AuthenticateAsync();
            var response = await TestClient.GetAsync($"UserProfile/{userId}/profileImage");
            var responseContent = await response.Content.ReadAsByteArrayAsync();
            var result = new FileContentResult(responseContent, "image/jpeg");
            result.FileContents
                .Should()
                .NotBeEmpty();
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