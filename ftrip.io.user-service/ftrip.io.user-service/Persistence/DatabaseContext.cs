using ftrip.io.framework.Contexts;
using ftrip.io.framework.Persistence.Sql.Database;
using ftrip.io.user_service.Accounts.Domain;
using ftrip.io.user_service.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace ftrip.io.user_service.Persistence
{
    public class DatabaseContext : DatabaseContextBase<DatabaseContext>
    {
        public DbSet<CredentialsAccount> CredentialsAccounts { get; set; }
        public DbSet<User> Users { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options, CurrentUserContext currentUserContext) :
            base(options, currentUserContext)
        {
        }
    }
}