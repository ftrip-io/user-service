using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Reservations;
using ftrip.io.user_service.Users.Domain;
using ftrip.ip.user_service.contracts.Users.Events;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.DeleteUser
{
    public class DeleteHostRequestHandler : IRequestHandler<DeleteHostRequest, User>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IReservationsQueryHelper _reservationsQueryHelper;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public DeleteHostRequestHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IReservationsQueryHelper reservationsQueryHelper,
            IMessagePublisher messagePublisher,
            IStringManager stringManager,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _reservationsQueryHelper = reservationsQueryHelper;
            _messagePublisher = messagePublisher;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<User> Handle(DeleteHostRequest request, CancellationToken cancellationToken)
        {
            await Validate(request, cancellationToken);

            await _unitOfWork.Begin(cancellationToken);

            var deletedUser = await _userRepository.Delete(request.HostId, cancellationToken);

            await _unitOfWork.Rollback(cancellationToken);

            await PublishUserDeletedEvent(deletedUser, cancellationToken);

            return deletedUser;
        }

        private async Task Validate(DeleteHostRequest request, CancellationToken cancellationToken)
        {
            var hostId = request.HostId;
            var activeReservationsCount = await _reservationsQueryHelper.CountActiveForHost(hostId, cancellationToken);
            if (activeReservationsCount > 0)
            {
                _logger.Error(
                    "Unable to delete guest because of active reservations - GuestId[{GuestId}], Count[{Count}]",
                    hostId, activeReservationsCount
                );
                throw new BadLogicException(_stringManager.Format("Users_CannotDeleteBecauseOfActiveReservations", hostId));
            }
        }

        private async Task PublishUserDeletedEvent(User user, CancellationToken cancellationToken)
        {
            var userDeleted = new UserDeletedEvent()
            {
                UserId = user.Id.ToString(),
                UserType = user.Type.ToString()
            };

            await _messagePublisher.Send<UserDeletedEvent, string>(userDeleted, cancellationToken);
        }
    }
}