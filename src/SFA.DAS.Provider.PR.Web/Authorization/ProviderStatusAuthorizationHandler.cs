using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.PR.Web.Authorization;

public class ProviderStatusAuthorizationHandler(IOuterApiClient _outerApiClient, IConfiguration _configuration, ProviderSharedUIConfiguration _providerSharedUiConfiguration) : AuthorizationHandler<ProviderStatusRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderStatusRequirement requirement)
    {
        HttpContext? currentContext;
        switch (context.Resource)
        {
            case HttpContext resource:
                currentContext = resource;
                break;
            case AuthorizationFilterContext authorizationFilterContext:
                currentContext = authorizationFilterContext.HttpContext;
                break;
            default:
                currentContext = null;
                break;
        }

        if (!context.User.HasClaim(c => c.Type.Equals(ProviderClaims.ProviderUkprn)))
        {
            context.Fail();
            return;
        }

        var claimValue = context.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn))?.Value;

        if (!int.TryParse(claimValue, out var ukprn))
        {
            context.Fail();
            return;
        }

        if (_configuration.IsStubAuthEnabled())
        {
            context.Succeed(requirement);
            return;
        }

        var response = await _outerApiClient.GetProviderStatus(ukprn, CancellationToken.None);

        if (!response.CanAccessService)
        {
            currentContext?.Response.Redirect($"{_providerSharedUiConfiguration.DashboardUrl}/error/403/invalid-status");
        }

        context.Succeed(requirement);
    }
}
