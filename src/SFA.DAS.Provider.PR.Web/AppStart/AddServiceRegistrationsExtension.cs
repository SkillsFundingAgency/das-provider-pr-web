using RestEase.HttpClientFactory;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.PR.Web.AppStart;

public static class AddServiceRegistrationsExtension
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfigurationRoot configuration)
    {
        AddConfigurationOptions(services, configuration);
        var outerApiConfiguration = configuration.GetSection(nameof(ProviderPROuterApiConfiguration)).Get<ProviderPROuterApiConfiguration>();
        AddOuterApi(services, outerApiConfiguration!);
        services.AddTransient<ISessionService, SessionService>();

        return services;
    }

    private static void AddOuterApi(this IServiceCollection services, ProviderPROuterApiConfiguration configuration)
    {
        services.AddTransient<IApimClientConfiguration>((_) => configuration);

        services.AddScoped<Http.MessageHandlers.DefaultHeadersHandler>();
        services.AddScoped<Http.MessageHandlers.LoggingMessageHandler>();
        services.AddScoped<Http.MessageHandlers.ApimHeadersHandler>();

        services
            .AddRestEaseClient<IOuterApiClient>(configuration.ApiBaseUrl)
            .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
            .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
            .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();
    }

    private static void AddConfigurationOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<ProviderSharedUIConfiguration>(configuration.GetSection(nameof(ProviderSharedUIConfiguration)));
        services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));
    }
}
