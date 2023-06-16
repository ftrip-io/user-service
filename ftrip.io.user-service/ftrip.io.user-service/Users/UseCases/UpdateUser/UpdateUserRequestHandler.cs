using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.contracts.Users.Events;
using ftrip.io.user_service.Users.Domain;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.UpdateUser
{
    public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, User>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IUserQueryHelper _userQueryHelper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger _logger;

        public UpdateUserRequestHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IUserQueryHelper userQueryHelper,
            IMessagePublisher messagePublisher,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _userQueryHelper = userQueryHelper;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<User> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var existingUser = await _userQueryHelper.ReadOrThrow(request.Id, cancellationToken);
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Email = request.Email;
            existingUser.City = request.City;

            await UpdateUser(existingUser, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            await PublishUserUpdatedEvent(existingUser, cancellationToken);

            return existingUser;
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