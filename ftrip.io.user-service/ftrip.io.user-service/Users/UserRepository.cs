using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using ftrip.io.user_service.Users.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ftrip.io.user_service.Users
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<IEnumerable<User>> ReadByIds(Guid[] ids, CancellationToken cancellationToken);
    }

    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> ReadByIds(Guid[] ids, CancellationToken cancellationToken)
        {
            return await _entities
                .Where(user => ids.Contains(user.Id))
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}