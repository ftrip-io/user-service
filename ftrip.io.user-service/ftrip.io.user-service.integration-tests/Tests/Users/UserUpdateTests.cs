using FluentAssertions;
using ftrip.io.user_service.Users.Domain;
using ftrip.io.user_service.Users.UseCases.UpdateUser;
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
        public async Task UpdateUser_NotAuthenticated_ReturnsUnauthorized()
        {
            // Arrange
            var id = UsersSeeder.ExistingUser.Id;
            var request = GetUpdateUserRequest();

            // Act
            var client = _apiFactory.CreateClient();
            var response = await client.PutAsJsonAsync(UserContants.UpdateUserApi(id), request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ForDifferentUser_ReturnsForbidden()
        {
            // Arrange
            var id = UsersSeeder.ExistingUser.Id;
            var request = GetUpdateUserRequest();

            // Act
            var client = _apiFactory.CreateAuthenticatedClient(id.ToString(), UserType.Guest);
            var response = await client.PutAsJsonAsync(UserContants.UpdateUserApi(Guid.NewGuid()), request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ForNonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = GetUpdateUserRequest();

            // Act
            var client = _apiFactory.CreateAuthenticatedClient(id.ToString(), UserType.Guest);
            var response = await client.PutAsJsonAsync(UserContants.UpdateUserApi(id), request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOk()
        {
            // Arrange
            var id = UsersSeeder.ExistingUser.Id;
            var request = GetUpdateUserRequest();

            // Act
            var client = _apiFactory.CreateAuthenticatedClient(id.ToString(), UserType.Guest);
            var response = await client.PutAsJsonAsync(UserContants.UpdateUserApi(id), request);
            var updatedUser = await response.Content.ReadFromJsonAsync<User>();

            // Assert
            updatedUser.Id.Should().Be(id);
            updatedUser.FirstName.Should().Be(request.FirstName);
            updatedUser.LastName.Should().Be(request.LastName);
            updatedUser.Email.Should().Be(request.Email);
            updatedUser.City.Should().Be(request.City);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private UpdateUserRequest GetUpdateUserRequest()
        {
            return new UpdateUserRequest()
            {
                FirstName = "Nikola",
                LastName = "Nikolic",
                Email = "nikola.nikolic@test.com",
                City = "Novi Sad",
            };
        }
    }
}