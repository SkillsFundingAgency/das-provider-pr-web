using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.Provider.PR.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AddDataProtectionExtension
{
    public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection(nameof(ApplicationSettings))
            .Get<ApplicationSettings>();

        if (config != null
            && !string.IsNullOrEmpty(config.DataProtectionKeysDatabase)
            && !string.IsNullOrEmpty(config.RedisConnectionString))
        {
            var redisConnectionString = config.RedisConnectionString;
            var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

            var redis = ConnectionMultiplexer
                .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

            services.AddDataProtection()
                .SetApplicationName("das-provider-web")
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
        }

        return services;
    }
}
