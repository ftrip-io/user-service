using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using ftrip.io.user_service.Accounts.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Accounts
{
    public interface IAccountRepository : IRepository<CredentialsAccount, Guid>
    {
        Task<CredentialsAccount> ReadByUsername(string username, CancellationToken cancellationToken = default);

        Task<CredentialsAccount> ReadByUserId(Guid userId, CancellationToken cancellationToken = default);
    }

    public class AccountRepository : Repository<CredentialsAccount, Guid>, IAccountRepository
    {
        public AccountRepository(DbContext context) : base(context)
        {
        }

        public async Task<CredentialsAccount> ReadByUsername(string username, CancellationToken cancellationToken = default)
        {
            return await _entities.FirstOrDefaultAsync(account => account.Username == username, cancellationToken);
        }

        public async Task<CredentialsAccount> ReadByUserId(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _entities.FirstOrDefaultAsync(account => account.UserId == userId, cancellationToken);
        }
    }
}