using ftrip.io.framework.auth;
using ftrip.io.framework.Correlation;
using ftrip.io.framework.CQRS;
using ftrip.io.framework.ExceptionHandling.Extensions;
using ftrip.io.framework.Globalization;
using ftrip.io.framework.HealthCheck;
using ftrip.io.framework.Installers;
using ftrip.io.framework.Mapping;
using ftrip.io.framework.messaging.Installers;
using ftrip.io.framework.Metrics;
using ftrip.io.framework.Persistence.Sql.Mariadb;
using ftrip.io.framework.Proxies;
using ftrip.io.framework.Secrets;
using ftrip.io.framework.Swagger;
using ftrip.io.framework.Tracing;
using ftrip.io.framework.Utilities;
using ftrip.io.framework.Validation;
using ftrip.io.user_service.Installers;
using ftrip.io.user_service.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net.Http;

namespace ftrip.io.user_service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private bool InTestMode { get => Environment.GetEnvironmentVariable("IN_TEST_MODE") != null; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            if (!InTestMode)
            {
                InstallerCollection.With(
                    new SwaggerInstaller<Startup>(services),
                    new HealthCheckUIInstaller(services),
                    new EnviromentSecretsManagerInstaller(services),
                    new JwtAuthenticationInstaller(services),
                    new MariadbInstaller<DatabaseContext>(services),
                    new MariadbHealthCheckInstaller(services),
                    new RabbitMQInstaller<Startup>(services, RabbitMQInstallerType.Publisher | RabbitMQInstallerType.Consumer),
                    new TracingInstaller(services, (tracingSettings) =>
                    {
                        tracingSettings.ApplicationLabel = "users";
                        tracingSettings.ApplicationVersion = GetType().Assembly.GetName().Version?.ToString() ?? "unknown";
                        tracingSettings.MachineName = Environment.MachineName;
                    }),
                    new MetricsInstaller(services)
                ).Install();
            }

            InstallerCollection.With(
                new GlobalizationInstaller<Startup>(services),
                new AutoMapperInstaller<Startup>(services),
                new FluentValidationInstaller<Startup>(services),
                new CQRSInstaller<Startup>(services),
                new DependenciesIntaller(services),
                new CorrelationInstaller(services),
                new ProxyGeneratorInstaller(services)
            ).Install();

            services.AddHttpClient("BookingService", (HttpClient client) =>
            {
                client.BaseAddress = new Uri(EnvReader.GetEnvVariableOrThrow("BOOKING_SERVICE_URL"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseRouting();

            if (!InTestMode)
            {
                app.UseMetrics();
            }

            app.UseCors(policy => policy
                .WithOrigins(Environment.GetEnvironmentVariable("API_PROXY_URL"))
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCorrelation();
            app.UseFtripioGlobalExceptionHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (!InTestMode)
            {
                app.UseFtripioSwagger(Configuration.GetSection(nameof(SwaggerUISettings)).Get<SwaggerUISettings>());
                app.UseFtripioHealthCheckUI(Configuration.GetSection(nameof(HealthCheckUISettings)).Get<HealthCheckUISettings>());
            }
        }
    }
}