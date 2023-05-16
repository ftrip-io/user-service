using FluentAssertions;
using ftrip.io.user_service.Auth.UseCases.Authenticate;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.user_service.integration_tests.Tests.Users
{
    public partial class UserTests
    {
        [Fact]
        public async Task Authenticate_ForNonExistingUser_ReturnsUnauthorized()
        {
            // Arrange
            var request = GetAuthenticateRequest();
            request.Username = "NonExistingUsername";

            // Act
            var client = _apiFactory.CreateClient();
            var response = await client.PostAsJsonAsync(UserContants.AuthenticateUserApi(), request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Authenticate_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var request = GetAuthenticateRequest();
            request.Password = "InvalidPassword";

            // Act
            var client = _apiFactory.CreateClient();
            var response = await client.PostAsJsonAsync(UserContants.AuthenticateUserApi(), request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Authenticate_ReturnsOk()
        {
            // Arrange
            var request = GetAuthenticateRequest();

            // Act
            var client = _apiFactory.CreateClient();
            var response = await client.PostAsJsonAsync(UserContants.AuthenticateUserApi(), request);
            var authenticatedUser = await response.Content.ReadFromJsonAsync<AuthenticatedUser>();

            // Assert
            authenticatedUser.Token.Should().NotBeNullOrEmpty();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private AuthenticateRequest GetAuthenticateRequest()
        {
            return new AuthenticateRequest()
            {
                Username = UsersSeeder.ExistingAccount.Username,
                Password = "password"
            };
        }
    }
}