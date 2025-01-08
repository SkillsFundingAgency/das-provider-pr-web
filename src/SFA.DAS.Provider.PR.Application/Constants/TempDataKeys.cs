using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Provider.PR.Application.Constants;

[ExcludeFromCodeCoverage]
public static class TempDataKeys
{
    public const string AccountLegalEntityName = nameof(AccountLegalEntityName);
    public const string PermissionsRequestId = nameof(PermissionsRequestId);
    public const string RequestId = nameof(RequestId);

}
