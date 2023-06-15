using FluentAssertions;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.contracts.Users.Events;
using ftrip.io.user_service.Users;
using ftrip.io.user_service.Users.Domain;
using ftrip.io.user_service.Users.UseCases.UpdateUser;
using Moq;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.user_service.unit_tests.Users.UseCases.UpdateUser
{
    public class UpdateUserRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<IUserQueryHelper> _userQueryHelperMock = new Mock<IUserQueryHelper>();
        private readonly Mock<IMessagePublisher> _messagePublisherMock = new Mock<IMessagePublisher>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        private readonly UpdateUserRequestHandler _handler;

        public UpdateUserRequestHandlerTests()
        {
            _handler = new UpdateUserRequestHandler(
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object,
                _userQueryHelperMock.Object,
                _messagePublisherMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void Handle_UserDoesNotExist_ThrowsException()
        {
            // Arrange
            var request = GetUpdateUserRequest();

            _userQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.Is((Guid id) => id == request.Id), It.IsAny<CancellationToken>()))
                .Throws(new MissingEntityException());

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Successful_ReturnsUser()
        {
            // Arrange
            var request = GetUpdateUserRequest();

            _userQueryHelperMock
                .Setup(qh => qh.ReadOrThrow(It.Is((Guid id) => id == request.Id), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) =>
                    Task.FromResult(new User()
                    {
                        Id = id,
                        FirstName = "Test",
                        LastName = "Testic",
                        Email = "test@test.com",
                        City = "Test"
                    }));

            _userRepositoryMock
               .Setup(r => r.Update(It.IsAny<User>(), It.IsAny<CancellationToken>()))
               .Returns((User u, CancellationToken _) => Task.FromResult(u));

            // Act
            var updatedUser = await _handler.Handle(request, CancellationToken.None);

            // Assert
            updatedUser.Should().NotBeNull();
            updatedUser.Id.Should().Be(request.Id);
            updatedUser.FirstName.Should().Be(request.FirstName);
            updatedUser.LastName.Should().Be(request.LastName);
            updatedUser.Email.Should().Be(request.Email);
            updatedUser.City.Should().Be(request.City);
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
            _messagePublisherMock.Verify(mp => mp.Send<UserUpdatedEvent, string>(It.IsAny<UserUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private UpdateUserRequest GetUpdateUserRequest()
        {
            return new UpdateUserRequest()
            {
                Id = Guid.NewGuid(),
                FirstName = "Test Update",
                LastName = "Testic Update",
                Email = "test-update@test.com",
                City = "Test Update",
            };
        }
    }
}