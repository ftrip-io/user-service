using ftrip.io.framework.ExceptionHandling.Exceptions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Accounts.Utilities;
using MediatR;
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

        public ChangePasswordRequestHandler(
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IStringManager stringManager)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _stringManager = stringManager;
        }

        public async Task<CredentialsAccount> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.Begin(cancellationToken);

            var account = await ReadByUserIdOrThrow(request.UserId, cancellationToken);
            ValidateCurrentPassword(account, request.CurrentPassword);

            account.Salt = RandomStringGenerator.Generate(32);
            account.HashedPassword = PasswordHasher.Hash(request.NewPassword, account.Salt);

            await _accountRepository.Update(account, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return account;
        }

        private async Task<CredentialsAccount> ReadByUserIdOrThrow(Guid userId, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.ReadByUserId(userId, cancellationToken);
            if (account == null)
            {
                throw new MissingEntityException(_stringManager.Format("Common_MissingEntity", userId));
            }

            return account;
        }

        private void ValidateCurrentPassword(CredentialsAccount account, string currentPassword)
        {
            var passwordMatches = PasswordHasher.Hash(currentPassword, account.Salt) == account.HashedPassword;
            if (!passwordMatches)
            {
                throw new BadLogicException(_stringManager.GetString("Accounts_Validation_InvalidCurrentPassword"));
            }
        }
    }
}