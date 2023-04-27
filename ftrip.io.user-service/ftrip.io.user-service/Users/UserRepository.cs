using ftrip.io.framework.Persistence.Contracts;
using ftrip.io.framework.Persistence.Sql.Repository;
using ftrip.io.user_service.Users.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace ftrip.io.user_service.Users
{
    public interface IUserRepository : IRepository<User, Guid>
    {
    }

    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }
    }
}