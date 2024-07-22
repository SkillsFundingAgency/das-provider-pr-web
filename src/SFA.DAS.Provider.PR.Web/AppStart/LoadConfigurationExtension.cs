using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;

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
            options.PreFixConfigurationKeys = false;
        });

        var configuration = configBuilder.Build();

        services.AddOptions();

        services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));

        return configuration;
    }
}
