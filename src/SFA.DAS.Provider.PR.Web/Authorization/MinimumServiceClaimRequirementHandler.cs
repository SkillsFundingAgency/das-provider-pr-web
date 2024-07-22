using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Provider.PR.Web.Extensions;

namespace SFA.DAS.Provider.PR.Web.Authorization;

[ExcludeFromCodeCoverage]
public class MinimumServiceClaimRequirementHandler : AuthorizationHandler<MinimumServiceClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumServiceClaimRequirement requirement)
    {
        if (context.User.HasPermission(requirement.MinimumServiceClaim)) context.Succeed(requirement);
        else context.Fail();

        return Task.CompletedTask;
    }
}
