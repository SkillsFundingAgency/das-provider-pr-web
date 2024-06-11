using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;

namespace SFA.DAS.Provider.PR.Web.AppStart;

public static class AddHealthChecksExtension
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        ApplicationSettings config = configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>()!;
        services
            .AddHealthChecks()
            .AddCheck<OuterApiHealthCheck>("Outer api health check")
            .AddRedis(config.RedisConnectionString, "Redis health check");
        return services;
    }
}
