using ftrip.io.framework.auth;
using ftrip.io.framework.Contexts;
using ftrip.io.framework.Installers;
using ftrip.io.framework.Secrets;
using Microsoft.Extensions.DependencyInjection;
using WebMotions.Fake.Authentication.JwtBearer;

namespace ftrip.io.user_service.integration_tests.Installers
{
    public class FakeJwtInstaller : IInstaller
    {
        private readonly IServiceCollection _services;

        public FakeJwtInstaller(IServiceCollection services)
        {
            _services = services;
        }

        public void Install()
        {
            _services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
            }).AddFakeJwtBearer();

            _services.AddControllers(c => c.Filters.Add<FillCurrentUserContextFilter>());
            _services.AddSingleton<ISecretsManager, EnviromentSecretsManager>();
            _services.AddScoped(typeof(CurrentUserContext));
        }
    }
}