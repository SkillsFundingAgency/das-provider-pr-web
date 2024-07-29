using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Web.Models;

public class EmployerPermissionViewModel
{
    public const string NoPermissionText = "No";
    public const string RecruitmentPermissionText = "Yes";
    public const string CohortsPermissionText = "Yes, employer will review records";
    public const string RecruitmentWithReviewPermissionText = "Yes, employer will review adverts";

    public required string Name { get; set; }
    public string? AgreementId { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public Guid? RequestId { get; set; }
    public required string CohortPermission { get; set; }
    public required string RecruitmentPermision { get; set; }

    public static implicit operator EmployerPermissionViewModel(ProviderRelationshipModel source)
        => new()
        {
            Name = source.EmployerName,
            AgreementId = source.AgreementId,
            AccountLegalEntityId = source.AccountLegalEntityId,
            RequestId = source.RequestId,
            CohortPermission = source.HasCreateCohortPermission ? CohortsPermissionText : NoPermissionText,
            RecruitmentPermision = GetRecruitmentPermissionText(source)
        };

    private static string GetRecruitmentPermissionText(ProviderRelationshipModel source)
    {
        if (source.HasRecruitmentWithReviewPermission) return RecruitmentWithReviewPermissionText;

        if (source.HasRecruitmentPermission) return RecruitmentPermissionText;

        return NoPermissionText;
    }
}
