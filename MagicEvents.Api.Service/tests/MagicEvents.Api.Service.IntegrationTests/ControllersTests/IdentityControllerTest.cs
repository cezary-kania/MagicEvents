using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.LoginUser;
using MagicEvents.Api.Service.Application.DTOs.Users.Identity.RegisterUser;
using MagicEvents.Api.Service.IntegrationTests.DataFactories;
using Newtonsoft.Json;
using Xunit;

namespace MagicEvents.Api.Service.IntegrationTests.ControllersTests
{
    public class IdentityControllerTest : IntegrationTest
    {
        [Fact]
        public async Task Register_WhenUserEmailIsNotInUse_ShouldCreateNewUser()
        {
            // Arrange
            var registerUserDto = UserTestDataFactory.CreateTestUser();
            var requestString = JsonConvert.SerializeObject(registerUserDto);
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PostAsync("Identity/register", content); 
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Register_WhenUserEmailIsInUse_ShouldReturnBadRequest()
        {
            // Arrange
            var firstUserDto = UserTestDataFactory.CreateTestUser();
            var requestString = JsonConvert.SerializeObject(firstUserDto);
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            await TestClient.PostAsync("Identity/register", content);

            var registerUserDto = UserTestDataFactory.CreateTestUser();
            registerUserDto.Email = firstUserDto.Email;
            requestString = JsonConvert.SerializeObject(registerUserDto);
            content = new StringContent(requestString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PostAsync("Identity/register", content); 
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WhenUserEmailIsNotCorrect_ShouldReturnBadRequest()
        {
            // Arrange
            var registerUserDto = UserTestDataFactory.CreateTestUser();
            registerUserDto.Email = "wrongemail@";
            var requestString = JsonConvert.SerializeObject(registerUserDto);
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PostAsync("Identity/register", content); 
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WhenUserPasswordIsNotCorrect_ShouldReturnBadRequest()
        {
            // Arrange
            var registerUserDto = UserTestDataFactory.CreateTestUser();
            registerUserDto.Password = "wrongpass";
            var requestString = JsonConvert.SerializeObject(registerUserDto);
            var content = new StringContent(requestString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PostAsync("Identity/register", content); 
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WhenUserCredentialsAreCorrect_ShouldReturnOK()
        {
            // Arrange
            var registerUserDto = UserTestDataFactory.CreateTestUser();
            var registerRequestString = JsonConvert.SerializeObject(registerUserDto);
            var registerContent = new StringContent(registerRequestString, Encoding.UTF8, "application/json");
            await TestClient.PostAsync("Identity/register", registerContent);

            var loginUserDto = new LoginUserDto
            {
                Email = registerUserDto.Email,
                Password = registerUserDto.Password
            };
            var loginRequestString = JsonConvert.SerializeObject(loginUserDto);
            var loginContent = new StringContent(loginRequestString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PostAsync("Identity/login", loginContent); 
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_WhenUserEmailIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            var registerUserDto = UserTestDataFactory.CreateTestUser();
            var registerRequestString = JsonConvert.SerializeObject(registerUserDto);
            var registerContent = new StringContent(registerRequestString, Encoding.UTF8, "application/json");
            await TestClient.PostAsync("Identity/register", registerContent);

            var loginUserDto = new LoginUserDto
            {
                Email = "test@test.com",
                Password = registerUserDto.Password
            };
            var loginRequestString = JsonConvert.SerializeObject(loginUserDto);
            var loginContent = new StringContent(loginRequestString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PostAsync("Identity/login", loginContent); 
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WhenUserPasswordIsNotValid_ShouldReturnBadRequest()
        {
            // Arrange
            var registerUserDto = UserTestDataFactory.CreateTestUser();
            var registerRequestString = JsonConvert.SerializeObject(registerUserDto);
            var registerContent = new StringContent(registerRequestString, Encoding.UTF8, "application/json");
            await TestClient.PostAsync("Identity/register", registerContent);

            var loginUserDto = new LoginUserDto
            {
                Email = registerUserDto.Email,
                Password = "InvalidP4$$W0rd"
            };
            var loginRequestString = JsonConvert.SerializeObject(loginUserDto);
            var loginContent = new StringContent(loginRequestString, Encoding.UTF8, "application/json");
            // Act
            var response = await TestClient.PostAsync("Identity/login", loginContent); 
            // Assert
            response.StatusCode
                .Should()
                .BeEquivalentTo(HttpStatusCode.BadRequest);
        }
    }
}