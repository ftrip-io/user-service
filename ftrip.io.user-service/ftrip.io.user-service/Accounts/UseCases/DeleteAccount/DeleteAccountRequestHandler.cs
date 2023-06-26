using ftrip.io.user_service.Accounts.Domain;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Accounts.UseCases.DeleteAccount
{
    public class DeleteAccountRequestHandler : IRequestHandler<DeleteAccountRequest, CredentialsAccount>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger _logger;

        public DeleteAccountRequestHandler(
            IAccountRepository accountRepository,
            ILogger logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<CredentialsAccount> Handle(DeleteAccountRequest request, CancellationToken cancellationToken)
        {
            var deletedAccount = await _accountRepository.Delete(request.AccountId, cancellationToken);

            _logger.Information(
                "Account deleted - AccountId[{AccountId}], UserId[{UserId}], Username[{Username}]",
                deletedAccount.Id, deletedAccount.UserId, deletedAccount.Username
            );

            return deletedAccount;
        }
    }
}