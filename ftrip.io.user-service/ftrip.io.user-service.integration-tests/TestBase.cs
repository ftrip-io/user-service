using Xunit;

namespace ftrip.io.user_service.integration_tests
{
    public class TestBase : IClassFixture<UserServiceApiFactory>
    {
        protected readonly UserServiceApiFactory _apiFactory;

        public TestBase(UserServiceApiFactory apiFactory)
        {
            _apiFactory = apiFactory;
        }
    }
}