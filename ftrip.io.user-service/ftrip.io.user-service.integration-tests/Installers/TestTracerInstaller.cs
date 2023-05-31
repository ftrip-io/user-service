using ftrip.io.framework.Installers;
using ftrip.io.framework.Tracing;
using Microsoft.Extensions.DependencyInjection;

namespace ftrip.io.user_service.integration_tests.Installers
{
    public class TestTracerInstaller : IInstaller
    {
        private readonly IServiceCollection _services;

        public TestTracerInstaller(IServiceCollection services)
        {
            _services = services;
        }

        public void Install()
        {
            _services.AddSingleton<ITracer>(new Tracer("Test", "1.0.0"));
        }
    }
}