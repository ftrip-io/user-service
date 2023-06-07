using ftrip.io.user_service.Users.Domain;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users.UseCases.ReadByIds
{
    public class ReadByIdsQueryHandler : IRequestHandler<ReadByIdsQuery, IEnumerable<User>>
    {
        private readonly IUserRepository _userRepository;

        public ReadByIdsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> Handle(ReadByIdsQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.ReadByIds(request.Ids, cancellationToken);
        }
    }
}