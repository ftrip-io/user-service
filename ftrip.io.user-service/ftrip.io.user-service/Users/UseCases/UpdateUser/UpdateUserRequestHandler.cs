using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.contracts.Users.Events;
using ftrip.io.user_service.Users.Domain;
using MediatR;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.UpdateUser
{
    public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, User>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IStringManager _stringManager;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        public UpdateUserRequestHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IStringManager stringManager,
            IMessagePublisher messagePublisher,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _stringManager = stringManager;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<User> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingUser = await ReadOrThrow(request.Id, cancellationToken);
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Email = request.Email;
            existingUser.City = request.City;

            await UpdateUser(existingUser, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            await PublishUserUpdatedEvent(existingUser, cancellationToken);

            return existingUser;
        }

        private async Task<User> ReadOrThrow(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Read(userId, cancellationToken);
            if (user == null)
            {
                _logger.Error("Cannot update user because it is not found - UserId[{UserId}]", userId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", userId));
            }

            return user;
        }

        private async Task<User> UpdateUser(User user, CancellationToken cancellationToken)
        {
            var updatedUser = await _userRepository.Update(user, cancellationToken);

            _logger.Information("User updated - UserId[{UserId}]", updatedUser.Id);

            return updatedUser;
        }

        private async Task PublishUserUpdatedEvent(User user, CancellationToken cancellationToken)
        {
            var userUpdated = new UserUpdatedEvent()
            {
                UserId = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            await _messagePublisher.Send<UserUpdatedEvent, string>(userUpdated, cancellationToken);
        }
    }
}