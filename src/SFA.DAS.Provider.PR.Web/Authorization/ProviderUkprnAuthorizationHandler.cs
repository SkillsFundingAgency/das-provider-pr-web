using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Provider.PR.Web.Extensions;
using SFA.DAS.Provider.PR.Web.Infrastructure;

namespace SFA.DAS.Provider.PR.Web.Authorization;

[ExcludeFromCodeCoverage]
public class ProviderUkprnAuthorizationHandler(IHttpContextAccessor _httpContextAccessor) : AuthorizationHandler<ProviderUkprnRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderUkprnRequirement requirement)
    {
        if (_httpContextAccessor.HttpContext == null || IsProviderAuthorised(_httpContextAccessor.HttpContext))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        context.Fail();
        return Task.CompletedTask;
    }

    private static bool IsProviderAuthorised(HttpContext context)
    {
        if (!(context.User.Identity?.IsAuthenticated).GetValueOrDefault()) return false;

        var ukprnFromUrl = context.Request.RouteValues[RouteValues.Ukprn]?.ToString();

        if (string.IsNullOrWhiteSpace(ukprnFromUrl)) return true;

        var ukprnClaim = context.User.GetUkprn()!;

        return ukprnClaim.Equals(ukprnFromUrl);
    }
}
