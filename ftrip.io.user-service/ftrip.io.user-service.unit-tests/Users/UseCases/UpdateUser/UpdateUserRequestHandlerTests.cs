using FluentAssertions;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Users;
using ftrip.io.user_service.Users.Domain;
using ftrip.io.user_service.Users.UseCases.UpdateUser;
using Moq;
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
        private readonly Mock<IStringManager> _stringManagerMock = new Mock<IStringManager>();

        private readonly UpdateUserRequestHandler _handler;

        public UpdateUserRequestHandlerTests()
        {
            _handler = new UpdateUserRequestHandler(
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object,
                _stringManagerMock.Object
            );
        }

        [Fact]
        public void Handle_UserDoesNotExist_ThrowsException()
        {
            // Assert
            var request = GetUpdateUserRequest();

            // Act
            Func<Task> handleAction = () => _handler.Handle(request, CancellationToken.None);

            // Assert
            handleAction.Should().ThrowExactlyAsync<MissingEntityException>();
            _unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Successful_ReturnsUser()
        {
            // Assert
            var request = GetUpdateUserRequest();

            _userRepositoryMock
                .Setup(r => r.Read(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns((Guid id, CancellationToken _) =>
                    Task.FromResult(new User()
                    {
                        Id = id,
                        FirstName = "Test",
                        LastName = "Testic",
                        Email = "test@test.com",
                        City = "Test"
                    })
                );

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