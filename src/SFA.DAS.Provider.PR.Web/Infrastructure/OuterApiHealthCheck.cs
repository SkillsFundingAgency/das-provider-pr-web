using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Provider.PR.Domain.Interfaces;

namespace SFA.DAS.Provider.PR.Web.Infrastructure;

public class OuterApiHealthCheck(IOuterApiClient _outerApiClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var response = await _outerApiClient.Ping();

        return response.IsSuccessStatusCode ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }
}
