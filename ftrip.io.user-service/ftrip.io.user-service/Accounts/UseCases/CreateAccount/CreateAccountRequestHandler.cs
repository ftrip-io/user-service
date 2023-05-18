using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.Utilities;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Accounts.UseCases.CreateAccount
{
    public class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest, CredentialsAccount>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public CreateAccountRequestHandler(
            IAccountRepository accountRepository,
            IStringManager stringManager,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<CredentialsAccount> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            await ValidateUsernameUniqueness(request.Username, cancellationToken);
            return await Create(request, cancellationToken);
        }

        private async Task ValidateUsernameUniqueness(string username, CancellationToken cancellationToken)
        {
            var existingAccount = await _accountRepository.ReadByUsername(username, cancellationToken);
            if (existingAccount != null)
            {
                _logger.Error("Account is already taken - Username[{Username}]", username);
                throw new BadLogicException(string.Format(_stringManager.GetString("Accounts_Validation_DuplicateUsername"), username));
            }
        }

        private async Task<CredentialsAccount> Create(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var account = new CredentialsAccount()
            {
                UserId = request.UserId,
                Username = request.Username,
                Salt = RandomStringGenerator.Generate(32)
            };

            account.HashedPassword = PasswordHasher.Hash(request.Password, account.Salt);

            var createdAccount = await _accountRepository.Create(account, cancellationToken);

            _logger.Information(
                "Account created - AccountId[{AccountId}], UserId[{UserId}], Username[{Username}]",
                createdAccount.Id, createdAccount.UserId, createdAccount.Username
            );

            return createdAccount;
        }
    }
}