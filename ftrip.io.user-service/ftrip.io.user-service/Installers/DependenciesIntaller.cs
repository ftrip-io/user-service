using ftrip.io.framework.Installers;
using ftrip.io.framework.Proxies;
using ftrip.io.user_service.Accounts;
using ftrip.io.user_service.Users;
using Microsoft.Extensions.DependencyInjection;

namespace ftrip.io.user_service.Installers
{
    public class DependenciesIntaller : IInstaller
    {
        private readonly IServiceCollection _services;

        public DependenciesIntaller(IServiceCollection services)
        {
            _services = services;
        }

        public void Install()
        {
            _services.AddProxiedScoped<IAccountRepository, AccountRepository>();
            _services.AddProxiedScoped<IUserRepository, UserRepository>();
        }
    }
}