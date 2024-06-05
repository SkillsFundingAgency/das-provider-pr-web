using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;

namespace SFA.DAS.Provider.PR.Web.Authorization;

[ExcludeFromCodeCoverage]
public class ProviderUkprnAuthorizationHandler(IHttpContextAccessor _httpContextAccessor) : AuthorizationHandler<ProviderUkprnRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderUkprnRequirement requirement)
    {
        //HttpContext currentContext = context?.Resource switch
        //{
        //    null => null,
        //    HttpContext resource => resource,
        //    _ => null
        //};
        //switch (context.Resource)
        //{
        //    case HttpContext resource:
        //        currentContext = resource;
        //        break;
        //    case AuthorizationFilterContext authorizationFilterContext:
        //        currentContext = authorizationFilterContext.HttpContext;
        //        break;
        //    default:
        //        currentContext = null;
        //        break;
        //}


        if (_httpContextAccessor.HttpContext == null) return;

        if (!IsProviderAuthorised(_httpContextAccessor.HttpContext))
        {
            context.Fail();
            return;
        }


        context.Succeed(requirement);
    }


    private static bool IsProviderAuthorised(HttpContext context)
    {
        if (!context.User.HasClaim(c => c.Type.Equals(ProviderClaims.ProviderUkprn)))
        {
            return false;
        }

        var ukprnFromUrl = context.Request.RouteValues[RouteValues.Ukprn];

        if (ukprnFromUrl == null) return true;

        var ukprnClaim = context.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn))!.Value;

        return !string.IsNullOrEmpty(ukprnClaim) && ukprnClaim.Equals(ukprnFromUrl);
    }
}
