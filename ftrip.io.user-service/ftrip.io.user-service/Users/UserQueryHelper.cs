using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.user_service.Users.Domain;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users
{
    public interface IUserQueryHelper
    {
        Task<User> ReadOrThrow(Guid userId, CancellationToken cancellationToken);
    }

    public class UserQueryHelper : IUserQueryHelper
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public UserQueryHelper(
            IUserRepository userRepository,
            IStringManager stringManager,
            ILogger logger)
        {
            _userRepository = userRepository;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<User> ReadOrThrow(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Read(userId, cancellationToken);
            if (user == null)
            {
                _logger.Error("Cannot update user because it is not found - UserId[{UserId}]", userId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", userId));
            }

            return user;
        }
    }
}