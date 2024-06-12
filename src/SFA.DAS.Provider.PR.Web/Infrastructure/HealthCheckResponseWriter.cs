using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SFA.DAS.Provider.PR.Web.Infrastructure;

public static class HealthCheckResponseWriter
{
    public static Task WriteJsonResponse(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = "application/json";

        return httpContext.Response.WriteAsync(JsonSerializer.Serialize(result, new JsonSerializerOptions() { WriteIndented = true }));
    }
}
