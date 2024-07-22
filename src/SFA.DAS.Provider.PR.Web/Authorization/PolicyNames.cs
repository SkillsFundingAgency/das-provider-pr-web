using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Provider.PR.Web.Authorization;

[ExcludeFromCodeCoverage]
public static class PolicyNames
{
    public static string HasProviderAccount => nameof(HasProviderAccount);
    public static string HasContributorOrAbovePermission => nameof(HasContributorOrAbovePermission);
}
