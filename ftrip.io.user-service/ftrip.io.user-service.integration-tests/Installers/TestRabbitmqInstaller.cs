using ftrip.io.framework.Installers;
using ftrip.io.framework.messaging.Configurations;
using ftrip.io.framework.messaging.Publisher;
using ftrip.io.framework.messaging.Settings;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using Testcontainers.RabbitMq;

namespace ftrip.io.user_service.integration_tests.Installers
{
    public class TestRabbitmqSettings : RabbitMQSettings
    {
        private readonly int _port = new Random().Next(10000, 65000);

        public override string Server { get => "localhost"; }
        public override string Port { get => _port.ToString(); }
        public override string User { get => RabbitMqBuilder.DefaultUsername; }
        public override string Password { get => RabbitMqBuilder.DefaultPassword; }
        public string Image { get => "rabbitmq:3-management-alpine"; }
    }

    public class TestRabbitmqInstaller : IInstaller
    {
        private readonly IServiceCollection _services;
        private readonly RabbitMQSettings _settings;

        public TestRabbitmqInstaller(IServiceCollection services, RabbitMQSettings settings)
        {
            _services = services;
            _settings = settings;
        }

        public void Install()
        {
            _services.AddSingleton(typeof(RabbitMQSettings), _settings);
            _services.AddSingleton<IMessagePublisher, MessagePublisher>();
            _services.AddSingleton(new QueuesForEvent());

            _services.AddMassTransit(x =>
            {
                x.AddConsumers(typeof(Startup).Assembly);
                x.SetKebabCaseEndpointNameFormatter();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.Host(new Uri(_settings.GetConnectionString()), h =>
                    {
                        h.Username(_settings.User);
                        h.Password(_settings.Password);
                    });

                    config.ConfigureEndpoints(provider);
                }));
            });

            _services.AddMassTransitHostedService();
        }
    }
}