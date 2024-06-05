using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;

namespace SFA.DAS.Provider.PR.Web.Authorization;

public static class AddAuthorizationPoliciesExtension
{
    private const string ProviderDaa = "DAA";
    private const string ProviderDab = "DAB";
    private const string ProviderDac = "DAC";
    private const string ProviderDav = "DAV";

    public static IServiceCollection AddAuthorizationServicePolicies(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, ProviderUkprnAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, ProviderStatusAuthorizationHandler>();

        //services.AddAuthorization<AuthorizationContextProvider>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.HasProviderAccount, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ProviderClaims.ProviderUkprn);
                policy.RequireClaim(ProviderClaims.Service, ProviderDaa, ProviderDab, ProviderDac, ProviderDav);
                policy.Requirements.Add(new ProviderUkprnRequirement());
                //policy.Requirements.Add(new ProviderStatusRequirement());
            });
        });

        return services;
    }
}
