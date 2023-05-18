using ftrip.io.framework.Logging;
using ftrip.io.framework.Persistence.Sql.Migrations;
using ftrip.io.user_service.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace ftrip.io.user_service
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .MigrateDbContext<DatabaseContext>()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilogLogging((hostingContext) =>
                {
                    return new LoggingOptions()
                    {
                        ApplicationName = hostingContext.HostingEnvironment.ApplicationName,
                        ApplicationLabel = "users",
                        ClientIdAttribute = "X-Forwarded-For",
                        GrafanaLokiUrl = Environment.GetEnvironmentVariable("GRAFANA_LOKI_URL")
                    };
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}