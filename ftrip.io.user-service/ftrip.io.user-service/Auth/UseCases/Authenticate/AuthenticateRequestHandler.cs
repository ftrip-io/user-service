using ftrip.io.framework.auth;
using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Secrets;
using ftrip.io.user_service.Accounts;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.Utilities;
using ftrip.io.user_service.Users;
using ftrip.io.user_service.Users.Domain;
using MediatR;
using Serilog;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Auth.UseCases.Authenticate
{
    public class AuthenticateRequestHandler : IRequestHandler<AuthenticateRequest, AuthenticatedUser>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStringManager _stringManager;
        private readonly ISecretsManager _secretsManager;
        private readonly ILogger _logger;

        public AuthenticateRequestHandler(
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IStringManager stringManager,
            ISecretsManager secretsManager,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _stringManager = stringManager;
            _secretsManager = secretsManager;
            _logger = logger;
        }

        public async Task<AuthenticatedUser> Handle(AuthenticateRequest request, CancellationToken cancellationToken)
        {
            var matchedAccount = await _accountRepository.ReadByUsername(request.Username, cancellationToken);
            if (matchedAccount == null)
            {
                _logger.Error("Cannot authenticate user because it is not found - Username[{Username}]", request.Username);
                throw new AuthorizationException(_stringManager.GetString("Accounts_Authenticate_Fails"));
            }

            if (!PasswordMatches(matchedAccount, request.Password))
            {
                _logger.Error("Cannot authenticate user because password is wrong - Username[{Username}]", request.Username);
                throw new AuthorizationException(_stringManager.GetString("Accounts_Authenticate_Fails"));
            }

            _logger.Information("User authenticated - Username[{Username}]", request.Username);

            var user = await _userRepository.Read(matchedAccount.UserId, cancellationToken);
            var token = GenerateJwtToken(user.Id, user.Type);
            return new AuthenticatedUser()
            {
                User = user,
                Token = token
            };
        }

        private bool PasswordMatches(CredentialsAccount account, string password)
        {
            return account.HashedPassword == PasswordHasher.Hash(password, account.Salt);
        }

        private string GenerateJwtToken(Guid id, UserType type)
        {
            return new JwtBuilder()
                .SetSecret(_secretsManager.Get("JWT_SECRET"))
                .SetTime(60)
                .AddClaim(ClaimTypes.Name, id)
                .AddClaim(ClaimTypes.Role, type)
                .Build();
        }
    }
}