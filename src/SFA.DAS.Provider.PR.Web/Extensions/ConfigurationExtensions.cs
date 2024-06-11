namespace SFA.DAS.Provider.PR.Web.Extensions;

public static class ConfigurationExtensions
{
    public static bool IsStubAuthEnabled(this IConfiguration configuration) => configuration["StubProviderAuth"] != null && configuration["StubProviderAuth"]!.Equals("true", StringComparison.CurrentCultureIgnoreCase);
}
