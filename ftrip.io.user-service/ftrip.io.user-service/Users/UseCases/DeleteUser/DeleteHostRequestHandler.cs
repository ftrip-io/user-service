using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.user_service.Reservations;
using ftrip.io.user_service.Users.Domain;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.DeleteUser
{
    public class DeleteHostRequestHandler : IRequestHandler<DeleteHostRequest, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IReservationsQueryHelper _reservationsQueryHelper;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public DeleteHostRequestHandler(
            IUserRepository userRepository,
            IReservationsQueryHelper reservationsQueryHelper,
            IStringManager stringManager,
            ILogger logger)
        {
            _userRepository = userRepository;
            _reservationsQueryHelper = reservationsQueryHelper;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<User> Handle(DeleteHostRequest request, CancellationToken cancellationToken)
        {
            await Validate(request, cancellationToken);

            return await _userRepository.Delete(request.HostId, cancellationToken);
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
    }
}