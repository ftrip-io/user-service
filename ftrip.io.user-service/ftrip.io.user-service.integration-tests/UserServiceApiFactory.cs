using ftrip.io.framework.Installers;
using ftrip.io.user_service.integration_tests.Installers;
using ftrip.io.user_service.Users.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ftrip.io.user_service.integration_tests
{
    public class UserServiceApiFactory : WebApplicationFactory<Startup>, IAsyncLifetime
    {
        protected readonly TestMariadbSettings _mariadbSettings;
        protected readonly TestRabbitmqSettings _rabbitmqSettings;

        protected readonly TestContainersCollection _testContainers;

        protected IServiceScope _serviceScope;

        public UserServiceApiFactory()
        {
            _mariadbSettings = new TestMariadbSettings();
            _rabbitmqSettings = new TestRabbitmqSettings();

            _testContainers = new TestContainersCollection()
            {
                TestContainers.BuildMariadbContainer(_mariadbSettings),
                TestContainers.BuildRabbitMqContainer(_rabbitmqSettings)
            };
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("IN_TEST_MODE", "true");

            builder.ConfigureServices((IServiceCollection services) =>
            {
                InstallerCollection.With(
                    new EnvironmentInstaller(),
                    new FakeJwtInstaller(services),
                    new TestMariadbInstaller(services, _mariadbSettings),
                    new TestRabbitmqInstaller(services, _rabbitmqSettings)
                ).Install();
            });
        }

        public new HttpClient CreateClient()
        {
            return base.CreateClient();
        }

        public HttpClient CreateAuthenticatedClient()
        {
            return CreateAuthenticatedClient(Guid.NewGuid().ToString(), null);
        }

        public HttpClient CreateAuthenticatedClient(string userId, UserType? userType)
        {
            var claims = new Dictionary<string, object>
            {
                { ClaimTypes.Name, userId },
                { ClaimTypes.Role, userType?.ToString() ?? "" },
            };

            var httpClient = base.CreateClient();
            httpClient.SetFakeBearerToken(claims);

            return httpClient;
        }

        public IServiceScope CreateServiceScope()
        {
            return Services.CreateScope();
        }

        public T GetService<T>()
        {
            var serviceScope = GetServiceScope();

            return serviceScope.ServiceProvider.GetService<T>();
        }

        protected IServiceScope GetServiceScope()
        {
            if (_serviceScope == null)
            {
                _serviceScope = CreateServiceScope();
            }

            return _serviceScope;
        }

        public async Task InitializeAsync()
        {
            await _testContainers.StartAll();
        }

        public async Task DisposeAsync()
        {
            await _testContainers.StopAll();
            Dispose();
        }
    }
}