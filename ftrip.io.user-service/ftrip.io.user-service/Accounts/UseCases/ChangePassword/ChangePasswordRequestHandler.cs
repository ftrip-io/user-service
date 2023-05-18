using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.Utilities;
using MediatR;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Accounts.UseCases.ChangePassword
{
    public class ChangePasswordRequestHandler : IRequestHandler<ChangePasswordRequest, CredentialsAccount>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IStringManager _stringManager;
        private readonly ILogger _logger;

        public ChangePasswordRequestHandler(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IStringManager stringManager,
            ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _stringManager = stringManager;
            _logger = logger;
        }

        public async Task<CredentialsAccount> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var account = await ReadByUserIdOrThrow(request.UserId, cancellationToken);
            ValidateCurrentPassword(account, request.CurrentPassword);

            account.Salt = RandomStringGenerator.Generate(32);
            account.HashedPassword = PasswordHasher.Hash(request.NewPassword, account.Salt);

            await UpdateAccount(account, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return account;
        }

        private async Task<CredentialsAccount> ReadByUserIdOrThrow(Guid userId, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.ReadByUserId(userId, cancellationToken);
            if (account == null)
            {
                _logger.Error("Cannot update password because user is not found - UserId[{UserId}]", userId);
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", userId));
            }

            return account;
        }

        private void ValidateCurrentPassword(CredentialsAccount account, string currentPassword)
        {
            var passwordMatches = PasswordHasher.Hash(currentPassword, account.Salt) == account.HashedPassword;
            if (!passwordMatches)
            {
                _logger.Error("Cannot update password because passed current is not valid - UserId[{UserId}]", account.UserId);
                throw new BadLogicException(_stringManager.GetString("Accounts_Validation_InvalidCurrentPassword"));
            }
        }

        private async Task UpdateAccount(CredentialsAccount account, CancellationToken cancellationToken)
        {
            await _accountRepository.Update(account, cancellationToken);

            _logger.Information("Password updated - UserId[{UserId}]", account.UserId);
        }
    }
}