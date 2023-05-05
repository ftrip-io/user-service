using FluentAssertions;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Accounts;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.UseCases.ChangePassword;
using ftrip.io.user_service.Accounts.Utilities;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.user_service.unit_tests.Account.ChangePassword
{
    public class ChangePasswordRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IAccountRepository> _accountRepositoryMock = new Mock<IAccountRepository>();
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();

        private readonly ChangePasswordRequestHandler _handler;

        public ChangePasswordRequestHandlerTests()
        {
            _handler = new ChangePasswordRequestHandler(
                _unitOfWorkMock.Object,
                _accountRepositoryMock.Object,
                _stringManagerMock.Object
            );
        }

        [Fact]
        public void Handle_AccountDoesNotExist_ThrowsException()
        {
            // Assert
            var request = GetChangePasswordRequest();

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public void Handle_CurrentPasswordDoesNotMatch_ThrowsException()
        {
            // Assert
            var request = GetChangePasswordRequest();

            _accountRepositoryMock
                .Setup(r => r.ReadByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid userId, CancellationToken _) =>
                {
                    var salt = "RandomSalt";
                    return Task.FromResult(new CredentialsAccount()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Salt = salt,
                        HashedPassword = PasswordHasher.Hash("Password", salt)
                    });
                });

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<BadLogicException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Successful_ReturnsAccount()
        {
            // Assert
            var request = GetChangePasswordRequest();

            _accountRepositoryMock
                .Setup(r => r.ReadByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid userId, CancellationToken _) =>
                {
                    var salt = "RandomSalt";
                    return Task.FromResult(new CredentialsAccount()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Salt = salt,
                        HashedPassword = PasswordHasher.Hash(request.CurrentPassword, salt)
                    });
                });

            // Act
            var account = await _handler.Handle(request, CancellationToken.None);

            // Assert
            account.Should().NotBeNull();
            account.Salt.Should().NotBe("RandomSalt");
            account.HashedPassword.Should().NotBe(request.NewPassword);
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        }

        private ChangePasswordRequest GetChangePasswordRequest()
        {
            return new ChangePasswordRequest()
            {
                UserId = Guid.NewGuid(),
                CurrentPassword = "Test1234@",
                NewPassword = "Test12345@"
            };
        }
    }
}