using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Provider.PR.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public static class RouteNames
{
    public const string Home = nameof(Home);
    public const string Employers = nameof(Employers);
    public const string ProviderSignOut = "provider-signout";
    public const string AddEmployerStart = nameof(AddEmployerStart);
    public const string AddEmployerSearchByEmail = nameof(AddEmployerSearchByEmail);
    public const string AddEmployerMultipleAccounts = nameof(AddEmployerMultipleAccounts);
    public const string OneAccountNoRelationship = nameof(OneAccountNoRelationship);
    public const string AddPermissionsAndEmployer = nameof(AddPermissionsAndEmployer);
    public const string AddEmployerConfirmation = nameof(AddEmployerConfirmation);
    public const string AddEmployerSearchByPaye = nameof(AddEmployerSearchByPaye);
}
