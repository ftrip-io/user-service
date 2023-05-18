using FluentAssertions;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Secrets;
using ftrip.io.user_service.Accounts;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.Utilities;
using ftrip.io.user_service.Auth.UseCases.Authenticate;
using ftrip.io.user_service.Users;
using ftrip.io.user_service.Users.Domain;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.user_service.unit_tests.Auth.UseCases.Authenticate
{
    public class AuthenticateRequestHandlerTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock = new Mock<IAccountRepository>();
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();
        private readonly Mock<ISecretsManager> _secretsManagerMock = new Mock<ISecretsManager>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        private readonly AuthenticateRequestHandler _handler;

        public AuthenticateRequestHandlerTests()
        {
            _handler = new AuthenticateRequestHandler(
                _accountRepositoryMock.Object,
                _userRepositoryMock.Object,
                _stringManagerMock.Object,
                _secretsManagerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void Handle_AccountDoesNotExist_ThrowsException()
        {
            // Arrange
            var request = GetAuthenticateRequest();

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<AuthorizationException>();
        }

        [Fact]
        public void Handle_CurrentPasswordDoesNotMatch_ThrowsException()
        {
            // Arrange
            var request = GetAuthenticateRequest();

            _accountRepositoryMock
                .Setup(r => r.ReadByUsername(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string username, CancellationToken _) =>
                {
                    var salt = "RandomSalt";
                    return Task.FromResult(new CredentialsAccount()
                    {
                        Id = Guid.NewGuid(),
                        Username = username,
                        Salt = salt,
                        HashedPassword = PasswordHasher.Hash("Password", salt)
                    });
                });

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<AuthorizationException>();
        }

        [Fact]
        public async Task Handle_Successful_ReturnsAccount()
        {
            // Arrange
            var request = GetAuthenticateRequest();

            _accountRepositoryMock
                .Setup(r => r.ReadByUsername(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string username, CancellationToken _) =>
                {
                    var salt = "RandomSalt";
                    return Task.FromResult(new CredentialsAccount()
                    {
                        Id = Guid.NewGuid(),
                        Username = username,
                        Salt = salt,
                        HashedPassword = PasswordHasher.Hash(request.Password, salt)
                    });
                });

            _userRepositoryMock
                .Setup(r => r.Read(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid userId, CancellationToken _) => Task.FromResult(new User()
                {
                    Id = userId,
                    Type = UserType.Guest
                }));

            _secretsManagerMock
                .Setup(m => m.Get(It.Is<string>((string key) => key == "JWT_SECRET")))
                .Returns("THIS IS JUST TEST SECRET.");

            // Act
            var authenticatedUser = await _handler.Handle(request, CancellationToken.None);

            // Assert
            authenticatedUser.User.Should().NotBeNull();
            authenticatedUser.Token.Should().NotBeNull();
        }

        private AuthenticateRequest GetAuthenticateRequest()
        {
            return new AuthenticateRequest()
            {
                Username = "Test@",
                Password = "Test1234@"
            };
        }
    }
}