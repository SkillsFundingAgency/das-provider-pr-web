using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Encoding;

namespace SFA.DAS.Provider.PR.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class LoadConfigurationExtension
{
    public static IConfigurationRoot LoadConfiguration(this IConfiguration config, IServiceCollection services)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddConfiguration(config)
            .AddEnvironmentVariables();

        configBuilder.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = config["ConfigNames"]!.Split(",");
            options.StorageConnectionString = config["ConfigurationStorageConnectionString"];
            options.EnvironmentName = config["EnvironmentName"];
            options.ConfigurationKeysRawJsonResult = new[] { ConfigurationKeys.EncodingConfig };
            options.PreFixConfigurationKeys = false;
        });

        var configuration = configBuilder.Build();

        var encodingsConfiguration = configuration.GetSection(ConfigurationKeys.EncodingConfig).Value;

        var encodingConfig = JsonSerializer.Deserialize<EncodingConfig>(encodingsConfiguration!);
        services.AddSingleton(encodingConfig!);

        return configuration;
    }
}
