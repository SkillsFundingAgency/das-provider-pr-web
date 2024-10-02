using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using SFA.DAS.Encoding;

namespace SFA.DAS.Provider.PR.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AddConfigurationExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var encodingsConfiguration = configuration.GetSection(ConfigurationKeys.EncodingConfig).Value;

        var encodingConfig = JsonSerializer.Deserialize<EncodingConfig>(encodingsConfiguration!);
        services.AddSingleton(encodingConfig!);

        return services;
    }
}

[ExcludeFromCodeCoverage]
public static class ConfigurationKeys
{
    public const string EncodingConfig = "SFA.DAS.Encoding";
}