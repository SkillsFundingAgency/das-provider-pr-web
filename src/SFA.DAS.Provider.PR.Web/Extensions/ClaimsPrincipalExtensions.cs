using System.Security.Claims;
using SFA.DAS.Provider.PR.Web.Authorization;

namespace SFA.DAS.Provider.PR.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUkprn(this ClaimsPrincipal user) => user.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn))?.Value;
}
