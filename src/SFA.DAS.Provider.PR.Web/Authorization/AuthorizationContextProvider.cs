using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Authorization.Context;

namespace SFA.DAS.Provider.PR.Web.Authorization;

[ExcludeFromCodeCoverage]
public class AuthorizationContextProvider(IHttpContextAccessor _httpContextAssessor) : IAuthorizationContextProvider
{
    public IAuthorizationContext GetAuthorizationContext() => new AuthorizationContext();
}
