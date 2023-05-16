using FluentAssertions;
using ftrip.io.user_service.Accounts.UseCases.CreateAccount;
using ftrip.io.user_service.Users.Domain;
using ftrip.io.user_service.Users.UseCases.CreateUser;
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
        public async Task CreateUser_DuplicateUsername_ReturnsBadRequest()
        {
            // Arrange
            var request = GetCreateUserRequest();
            request.Account.Username = UsersSeeder.ExistingAccount.Username;

            // Act
            var client = _apiFactory.CreateClient();
            var response = await client.PostAsJsonAsync(UserContants.CreateUserApi(), request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_ReturnsOk()
        {
            // Arrange
            var request = GetCreateUserRequest();

            // Act
            var client = _apiFactory.CreateClient();
            var response = await client.PostAsJsonAsync(UserContants.CreateUserApi(), request);
            var createdUser = await response.Content.ReadFromJsonAsync<User>();

            // Assert
            createdUser.Id.Should().NotBeEmpty();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private CreateUserRequest GetCreateUserRequest()
        {
            return new CreateUserRequest()
            {
                FirstName = "Test",
                LastName = "Testic",
                Email = "test@test.com",
                City = "Test",
                Type = UserType.Guest,
                Account = new CreateAccountRequest()
                {
                    Username = "test",
                    Password = "Test1234@"
                }
            };
        }
    }
}