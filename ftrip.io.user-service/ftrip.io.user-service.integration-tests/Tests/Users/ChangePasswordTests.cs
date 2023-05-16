using FluentAssertions;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.UseCases.ChangePassword;
using ftrip.io.user_service.Users.Domain;
using System;
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
        public async Task ChangePassword_NotAuthenticated_ReturnsUnauthorized()
        {
            // Arrange
            var id = UsersSeeder.ChangePasswordUser.Id;
            var request = GetChangePasswordRequest();

            // Act
            var client = _apiFactory.CreateClient();
            var response = await client.PutAsJsonAsync(UserContants.ChanngePasswordForUserApi(id), request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_ForDifferentUser_ReturnsForbidden()
        {
            // Arrange
            var id = UsersSeeder.ChangePasswordUser.Id;
            var request = GetChangePasswordRequest();

            // Act
            var client = _apiFactory.CreateAuthenticatedClient(id.ToString(), UserType.Guest);
            var response = await client.PutAsJsonAsync(UserContants.ChanngePasswordForUserApi(Guid.NewGuid()), request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_ForNonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GetChangePasswordRequest();

            // Act
            var client = _apiFactory.CreateAuthenticatedClient(id.ToString(), UserType.Guest);
            var response = await client.PutAsJsonAsync(UserContants.ChanngePasswordForUserApi(id), request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_WithInvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var id = UsersSeeder.ChangePasswordUser.Id;
            var request = GetChangePasswordRequest();
            request.CurrentPassword = "InvalidPassword123";

            // Act
            var client = _apiFactory.CreateAuthenticatedClient(id.ToString(), UserType.Guest);
            var response = await client.PutAsJsonAsync(UserContants.ChanngePasswordForUserApi(id), request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk()
        {
            // Arrange
            var id = UsersSeeder.ChangePasswordUser.Id;
            var request = GetChangePasswordRequest();

            // Act
            var client = _apiFactory.CreateAuthenticatedClient(id.ToString(), UserType.Guest);
            var response = await client.PutAsJsonAsync(UserContants.ChanngePasswordForUserApi(id), request);
            var credentialsAccount = await response.Content.ReadFromJsonAsync<CredentialsAccount>();

            // Assert
            credentialsAccount.UserId.Should().Be(id);
            credentialsAccount.Salt.Should().NotBe(UsersSeeder.ChangePasswordAccount.Salt);
            credentialsAccount.HashedPassword.Should().NotBe(UsersSeeder.ChangePasswordAccount.HashedPassword);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private ChangePasswordRequest GetChangePasswordRequest()
        {
            return new ChangePasswordRequest()
            {
                CurrentPassword = "Password123",
                NewPassword = "NewPassoword123!"
            };
        }
    }
}