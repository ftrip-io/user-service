using AutoMapper;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Accounts.UseCases.CreateAccount;
using ftrip.io.user_service.Users.Domain;
using ftrip.io.user_service.contracts.Users.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.CreateUser
{
    public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, User>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IMessagePublisher _messagePublisher;

        public CreateUserRequestHandler(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IMapper mapper,
            IMediator mediator,
            IMessagePublisher messagePublisher)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;
            _messagePublisher = messagePublisher;
        }

        public async Task<User> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var user = _mapper.Map<User>(request);
            var createdUser = await CreateUser(user, cancellationToken);

            request.Account.UserId = createdUser.Id;
            await CreateAccount(request.Account, cancellationToken);

            await _unitOfWork.Commit(cancellationToken);

            await PublishUserCreatedEvent(createdUser, cancellationToken);

            return createdUser;
        }

        private async Task<User> CreateUser(User user, CancellationToken cancellationToken)
        {
            return await _userRepository.Create(user, cancellationToken);
        }

        private async Task CreateAccount(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
        }

        private async Task PublishUserCreatedEvent(User user, CancellationToken cancellationToken)
        {
            var userCreated = new UserCreatedEvent()
            {
                UserId = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            await _messagePublisher.Send<UserCreatedEvent, string>(userCreated, cancellationToken);
        }
    }
}