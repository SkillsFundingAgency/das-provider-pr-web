using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Provider.PR.Web.Authorization;

[ExcludeFromCodeCoverage]
public static class ProviderClaims
{
    public static string Ukprn => "http://schemas.portal.com/ukprn";
    public static string DisplayName => "http://schemas.portal.com/displayname";
    public static string Service => "http://schemas.portal.com/service";
}
