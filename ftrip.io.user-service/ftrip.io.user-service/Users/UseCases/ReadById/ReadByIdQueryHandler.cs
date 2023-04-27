using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.user_service.Users.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.ReadById
{
    public class ReadByIdQueryHandler : IRequestHandler<ReadByIdQuery, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringManager _stringManager;

        public ReadByIdQueryHandler(
            IUserRepository userRepository,
            IStringManager stringManager)
        {
            _userRepository = userRepository;
            _stringManager = stringManager;
        }

        public async Task<User> Handle(ReadByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Read(request.Id);
            if (user == null)
            {
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", request.Id));
            }

            return user;
        }
    }
}