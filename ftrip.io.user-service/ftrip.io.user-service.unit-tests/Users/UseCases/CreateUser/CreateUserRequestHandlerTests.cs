using AutoMapper;
using FluentAssertions;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Accounts.UseCases.CreateAccount;
using ftrip.io.user_service.Users;
using ftrip.io.user_service.Users.Domain;
using ftrip.io.user_service.Users.UseCases.CreateUser;
using ftrip.io.user_service.contracts.Users.Events;
using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Serilog;

namespace ftrip.io.user_service.unit_tests.Users.UseCases.CreateUser
{
    public class CreateUserRequestHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<IMessagePublisher> _messagePublisherMock = new Mock<IMessagePublisher>();
        private readonly Mock<ILogger> _loggerMock = new Mock<ILogger>();

        private readonly CreateUserRequestHandler _handler;

        public CreateUserRequestHandlerTests()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateUserRequest, User>();
            }).CreateMapper();

            _handler = new CreateUserRequestHandler(
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object,
                mapper,
                _mediatorMock.Object,
                _messagePublisherMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void Handle_CreatingAccountFails_ThrowsException()
        {
            // Arrange
            var request = GetCreateUserRequest();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateAccountRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new BadLogicException("Error while saving account."));

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<BadLogicException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Successful_ReturnsUser()
        {
            // Arrange
            var request = GetCreateUserRequest();

            _userRepositoryMock
                .Setup(r => r.Create(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns((User u, CancellationToken _) =>
                {
                    u.Id = Guid.NewGuid();
                    return Task.FromResult(u);
                });

            // Act
            var createdUser = await _handler.Handle(request, CancellationToken.None);

            // Assert
            createdUser.Should().NotBeNull();
            createdUser.Id.Should().Be(request.Account.UserId);
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
            _messagePublisherMock.Verify(mp => mp.Send<UserCreatedEvent, string>(It.IsAny<UserCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
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