using ftrip.io.framework.Persistence.Sql.Migrations;
using ftrip.io.user_service.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}