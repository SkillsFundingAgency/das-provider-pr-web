using Microsoft.AspNetCore.Authentication;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Extensions;

namespace SFA.DAS.Provider.PR.Web.AppStart;

public static class AddAuthenticationExtension
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.IsStubAuthEnabled())
        {
            services.AddAuthentication("Provider-stub").AddScheme<AuthenticationSchemeOptions, ProviderStubAuthHandler>("Provider-stub", options => { });
        }
        else
        {
            services.AddAndConfigureDfESignInAuthentication(configuration, "SFA.DAS.ProviderApprenticeshipService", typeof(CustomServiceRole), ClientName.ProviderRoatp, "/signout", string.Empty);
        }

        return services;
    }
}
