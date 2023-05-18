using FluentAssertions;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.user_service.Accounts;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.UseCases.CreateAccount;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.user_service.unit_tests.Accounts.UseCases.CreateAccount
{
    public class CreateAccountRequestHandlerTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock = new Mock<IAccountRepository>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        private readonly CreateAccountRequestHandler _handler;

        public CreateAccountRequestHandlerTests()
        {
            _handler = new CreateAccountRequestHandler(
                _accountRepositoryMock.Object,
                _stringManagerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void Handle_UsernameIsTaken_ThrowsException()
        {
            // Arrange
            var request = GetCreateAccountRequest();

            _accountRepositoryMock
                .Setup(r => r.ReadByUsername(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns((string username, CancellationToken _) =>
                    Task.FromResult(new CredentialsAccount()
                    {
                        Username = username
                    })
                );

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<BadLogicException>();
        }

        [Fact]
        public async Task Handle_Successful_ReturnsAccount()
        {
            // Arrange
            var request = GetCreateAccountRequest();

            _accountRepositoryMock
                .Setup(r => r.Create(It.IsAny<CredentialsAccount>(), It.IsAny<CancellationToken>()))
                .Returns((CredentialsAccount account, CancellationToken _) =>
                {
                    account.Id = Guid.NewGuid();
                    return Task.FromResult(account);
                });

            // Act
            var createdAccount = await _handler.Handle(request, CancellationToken.None);

            // Assert
            createdAccount.Should().NotBeNull();
            createdAccount.Salt.Should().NotBeNull();
            createdAccount.HashedPassword.Should().NotBe(request.Password);
        }

        private CreateAccountRequest GetCreateAccountRequest()
        {
            return new CreateAccountRequest()
            {
                Username = "Test",
                Password = "Test1234@"
            };
        }
    }
}