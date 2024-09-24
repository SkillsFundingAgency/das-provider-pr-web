using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;

namespace SFA.DAS.Provider.PR.Web.Models;

public class EmployerPermissionViewModel
{
    public required string Name { get; set; }
    public string? AgreementId { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public Guid? RequestId { get; set; }
    public required string CohortPermission { get; set; }
    public required string RecruitmentPermision { get; set; }
    public bool HasPendingRequest => RequestId != null;
    public bool HasAgreementId => !string.IsNullOrEmpty(AgreementId);

    public static implicit operator EmployerPermissionViewModel(ProviderRelationshipModel source)
        => new()
        {
            Name = source.EmployerName.ToUpper(),
            AgreementId = source.AgreementId,
            AccountLegalEntityId = source.AccountLegalEntityId,
            RequestId = source.RequestId,
            CohortPermission = source.HasCreateCohortPermission ? SetPermissionsText.CohortsPermissionText : SetPermissionsText.NoPermissionText,
            RecruitmentPermision = GetRecruitmentPermissionText(source)
        };

    private static string GetRecruitmentPermissionText(ProviderRelationshipModel source)
    {
        if (source.HasRecruitmentWithReviewPermission) return SetPermissionsText.RecruitmentWithReviewPermissionText;

        if (source.HasRecruitmentPermission) return SetPermissionsText.RecruitmentPermissionText;

        return SetPermissionsText.NoPermissionText;
    }
}
