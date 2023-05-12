using ftrip.io.framework.Installers;
using ftrip.io.framework.Persistence.Sql.Mariadb;
using ftrip.io.framework.Persistence.Sql.Settings;
using ftrip.io.user_service.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Testcontainers.MariaDb;

namespace ftrip.io.user_service.integration_tests.Installers
{
    public class TestMariadbSettings : SqlDatabaseSettings
    {
        private readonly int _port = new Random().Next(10000, 65000);

        public override string Server { get => "localhost"; }
        public override string Port { get => _port.ToString(); }
        public override string Database { get => MariaDbBuilder.DefaultDatabase; }
        public override string User { get => MariaDbBuilder.DefaultUsername; }
        public override string Password { get => MariaDbBuilder.DefaultPassword; }
        public string Image { get => "mariadb:10.7.8-focal"; }
    }

    public class TestMariadbInstaller : IInstaller
    {
        private readonly IServiceCollection _services;
        private readonly SqlDatabaseSettings _settings;

        public TestMariadbInstaller(IServiceCollection services, SqlDatabaseSettings settings)
        {
            _services = services;
            _settings = settings;
        }

        public void Install()
        {
            InstallerCollection.With(
                new MariadbInstaller<DatabaseContext>(_services, _settings),
                new MariadbHealthCheckInstaller(_services)
            ).Install();

            MigrateDatabase();
        }

        private void MigrateDatabase()
        {
            using var serviceProvider = _services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<DatabaseContext>();
            context.Database.Migrate();
        }
    }
}