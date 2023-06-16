using ftrip.io.user_service.Users.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.DeleteUser
{
    public class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest, User>
    {
        private readonly IUserQueryHelper _userQueryHelper;
        private readonly IMediator _mediator;
        private readonly IDictionary<UserType, Func<Guid, CancellationToken, Task<User>>> _deleteHandler;

        public DeleteUserRequestHandler(
            IUserQueryHelper userQueryHelper,
            IMediator mediator)
        {
            _userQueryHelper = userQueryHelper;
            _mediator = mediator;

            _deleteHandler = new Dictionary<UserType, Func<Guid, CancellationToken, Task<User>>>()
            {
                [UserType.Guest] = DeleteGuest,
                [UserType.Host] = DeleteHost
            };
        }

        public async Task<User> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userQueryHelper.ReadOrThrow(request.UserId, cancellationToken);

            return await _deleteHandler[user.Type](request.UserId, cancellationToken);
        }

        private async Task<User> DeleteGuest(Guid guestId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new DeleteGuestRequest() { GuestId = guestId });
        }

        private async Task<User> DeleteHost(Guid hostId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new DeleteHostRequest() { HostId = hostId });
        }
    }
}