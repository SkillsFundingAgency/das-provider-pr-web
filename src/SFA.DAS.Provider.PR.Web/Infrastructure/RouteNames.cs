﻿using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Provider.PR.Web.Infrastructure;

[ExcludeFromCodeCoverage]
public static class RouteNames
{
    public const string Home = nameof(Home);
    public const string ProviderSignOut = "provider-signout";
    public const string AddEmployerStart = nameof(AddEmployerStart);
    public const string AddEmployerSearchByEmail = nameof(AddEmployerSearchByEmail);
    public const string AddEmployerMultipleAccounts = nameof(AddEmployerMultipleAccounts);

}
