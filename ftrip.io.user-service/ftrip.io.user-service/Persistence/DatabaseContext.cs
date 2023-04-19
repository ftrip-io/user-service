using ftrip.io.framework.Contexts;
using ftrip.io.framework.Persistence.Sql.Database;
using Microsoft.EntityFrameworkCore;

namespace ftrip.io.user_service.Persistence
{
    public class DatabaseContext : DatabaseContextBase<DatabaseContext>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options, CurrentUserContext currentUserContext) :
            base(options, currentUserContext)
        {
        }
    }
}