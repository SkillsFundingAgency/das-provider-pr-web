using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Provider.PR.Web.Authorization;

[ExcludeFromCodeCoverage]
public class MinimumServiceClaimRequirement : IAuthorizationRequirement
{
    public ServiceClaim MinimumServiceClaim { get; set; }

    public MinimumServiceClaimRequirement(ServiceClaim minimumServiceClaim)
    {
        MinimumServiceClaim = minimumServiceClaim;
    }
}
