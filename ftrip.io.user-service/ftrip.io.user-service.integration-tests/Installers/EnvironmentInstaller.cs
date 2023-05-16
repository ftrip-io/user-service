using ftrip.io.framework.Installers;
using System;

namespace ftrip.io.user_service.integration_tests.Installers
{
    public class EnvironmentInstaller : IInstaller
    {
        public EnvironmentInstaller()
        {
        }

        public void Install()
        {
            Environment.SetEnvironmentVariable("API_PROXY_URL", "http://localhost");
            Environment.SetEnvironmentVariable("GRAFANA_LOKI_URL", "http://localhost");
            Environment.SetEnvironmentVariable("JWT_SECRET", "super test secret");
        }
    }
}