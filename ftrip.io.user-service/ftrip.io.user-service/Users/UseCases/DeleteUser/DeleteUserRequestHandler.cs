using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Accounts;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.UseCases.DeleteAccount;
using ftrip.io.user_service.contracts.Users.Events;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserQueryHelper _userQueryHelper;
        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMediator _mediator;
        private readonly IDictionary<UserType, Func<Guid, CancellationToken, Task<User>>> _deleteHandler;

        public DeleteUserRequestHandler(
            IUnitOfWork unitOfWork,
            IUserQueryHelper userQueryHelper,
            IAccountRepository accountRepository,
            IMessagePublisher messagePublisher,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _userQueryHelper = userQueryHelper;
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
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
            var account = await _accountRepository.ReadByUserId(request.UserId, cancellationToken);

            await _unitOfWork.Begin(cancellationToken);

            var deletedUser = await DeleteUser(user, cancellationToken);
            await DeleteAccount(account, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishUserDeletedEvent(deletedUser, cancellationToken);

            return deletedUser;
        }

        private async Task<User> DeleteUser(User user, CancellationToken cancellationToken)
        {
            return await _deleteHandler[user.Type](user.Id, cancellationToken);
        }

        private async Task<CredentialsAccount> DeleteAccount(CredentialsAccount account, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new DeleteAccountRequest() { AccountId = account.Id }, cancellationToken);
        }

        private async Task<User> DeleteGuest(Guid guestId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new DeleteGuestRequest() { GuestId = guestId });
        }

        private async Task<User> DeleteHost(Guid hostId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new DeleteHostRequest() { HostId = hostId });
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