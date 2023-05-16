using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using ftrip.io.user_service.integration_tests.Installers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.MariaDb;
using Testcontainers.RabbitMq;

namespace ftrip.io.user_service.integration_tests
{
    public static class TestContainers
    {
        public static MariaDbContainer BuildMariadbContainer(TestMariadbSettings settings)
        {
            return new MariaDbBuilder()
                .WithImage(settings.Image)
                .WithPortBinding(int.Parse(settings.Port), MariaDbBuilder.MariaDbPort)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MariaDbBuilder.MariaDbPort))
                .Build();
        }

        public static RabbitMqContainer BuildRabbitMqContainer(TestRabbitmqSettings settings)
        {
            return new RabbitMqBuilder()
                .WithImage(settings.Image)
                .WithPortBinding(int.Parse(settings.Port), RabbitMqBuilder.RabbitMqPort)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(RabbitMqBuilder.RabbitMqPort))
                .Build();
        }
    }

    public class TestContainersCollection : Collection<DockerContainer>
    {
        public async Task StartAll()
        {
            var starts = Items.Select(i => i.StartAsync());

            await Task.WhenAll(starts);
        }

        public async Task StopAll()
        {
            var starts = Items.Select(i => i.StartAsync());

            await Task.WhenAll(starts);
        }
    }
}