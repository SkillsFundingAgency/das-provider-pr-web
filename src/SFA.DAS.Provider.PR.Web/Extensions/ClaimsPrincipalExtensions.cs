using System.Security.Claims;
using SFA.DAS.Provider.PR.Web.Authorization;

namespace SFA.DAS.Provider.PR.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUkprn(this ClaimsPrincipal user) => user.FindFirst(c => c.Type.Equals(ProviderClaims.Ukprn))?.Value;

    public static bool HasPermission(this ClaimsPrincipal user, ServiceClaim minimumRequiredClaim)
    {
        var serviceClaims = user
            .FindAll(c => c.Type == ProviderClaims.Service)
            .Select(c => c.Value)
            .ToList();

        ServiceClaim? highestClaim = null;

        if (serviceClaims.Contains(ServiceClaim.DAA.ToString())) highestClaim = ServiceClaim.DAA;
        else if (serviceClaims.Contains(ServiceClaim.DAB.ToString())) highestClaim = ServiceClaim.DAB;
        else if (serviceClaims.Contains(ServiceClaim.DAC.ToString())) highestClaim = ServiceClaim.DAC;
        else if (serviceClaims.Contains(ServiceClaim.DAV.ToString())) highestClaim = ServiceClaim.DAV;

        return highestClaim.HasValue && highestClaim.Value >= minimumRequiredClaim;
    }
}
