using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;

namespace SFA.DAS.Provider.PR.Web.AppStart;

public static class AddSessionExtension
{
    public static IServiceCollection AddSession(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.IsEssential = true;
        });

        if (configuration.IsLocal())
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(options =>
            {
                var applicationConfiguration = configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>();
                options.Configuration = applicationConfiguration!.RedisConnectionString;
            });
        }

        return services;
    }

    public static bool IsLocal(this IConfiguration configuration) => configuration["EnvironmentName"]!.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
}
