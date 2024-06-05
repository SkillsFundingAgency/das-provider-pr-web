using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Provider.PR.Domain.Interfaces;
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

        var isStubProviderValidationEnabled = GetUseStubProviderValidationSetting();

        var response = await _outerApiClient.GetProviderStatus(ukprn, CancellationToken.None);

        // check if the stub is activated to by-pass the validation. Mostly used for local development purpose.
        // logic to check if the provider is authorized if not redirect the user to PAS 401 un-authorized page.
        if (!isStubProviderValidationEnabled && !response.CanAccessService)
        {
            currentContext?.Response.Redirect($"{_providerSharedUiConfiguration.DashboardUrl}/error/403/invalid-status");
        }

        context.Succeed(requirement);
    }

    private bool GetUseStubProviderValidationSetting()
    {
        var value = _configuration.GetSection("UseStubProviderValidation").Value;

        return value != null && bool.TryParse(value, out var result) && result;
    }
}
