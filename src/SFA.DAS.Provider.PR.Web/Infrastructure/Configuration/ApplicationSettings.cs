using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public class ApplicationSettings
{
    public int EmployersPageSize { get; set; }
    public required string RedisConnectionString { get; set; }
    public required string DataProtectionKeysDatabase { get; set; }
    public required string DfESignInServiceHelpUrl { get; set; }
}
