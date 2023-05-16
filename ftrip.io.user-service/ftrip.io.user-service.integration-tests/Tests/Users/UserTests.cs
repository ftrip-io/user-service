using ftrip.io.user_service.integration_tests.Seeding;
using ftrip.io.user_service.Persistence;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.user_service.integration_tests.Tests.Users
{
    public partial class UserTests : TestBase, IAsyncLifetime
    {
        private readonly Seeder _seeder;

        public UserTests(UserServiceApiFactory factory) :
            base(factory)
        {
            var context = factory.GetService<DatabaseContext>();

            _seeder = new Seeder(typeof(UsersSeeder), context);
        }

        public async Task InitializeAsync()
        {
            await _seeder.SeedAsync();
        }

        public async Task DisposeAsync()
        {
            await _seeder.UnseedAsync();
        }
    }
}