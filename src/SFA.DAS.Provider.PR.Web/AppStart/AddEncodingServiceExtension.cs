using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Encoding;

namespace SFA.DAS.Provider.PR.Web.AppStart;

[ExcludeFromCodeCoverage]
public static class AddEncodingServiceExtension
{
    public static IServiceCollection AddEncodingService(this IServiceCollection services)
    {
        services.AddSingleton<IEncodingService, EncodingService>();
        return services;
    }
}
