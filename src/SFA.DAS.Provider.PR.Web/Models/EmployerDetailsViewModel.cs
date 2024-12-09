using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;

namespace SFA.DAS.Provider.PR.Web.Models;
public class EmployerDetailsViewModel
{
    public long AccountLegalEntityId { get; set; }

    public string AccountLegalEntityPublicHashedId { get; set; } = null!;

    public string AccountLegalEntityName { get; set; } = null!;

    public long Ukprn { get; set; }

    public Operation[] Operations { get; set; } = [];

    public PermissionAction? LastAction { get; set; }

    public string LastActionDate { get; set; } = null!;

    public string ProviderName { get; set; } = null!;

    public bool HasExistingPermissions { get; set; } = true;

    public Operation[]? LastRequestOperations { get; set; } = [];

    public string[] CurrentPermissions => SetPermissionText(Operations);

    public string[] RequestedPermissions => SetPermissionText(LastRequestOperations ?? Array.Empty<Operation>());

    public string EmployersLink { get; set; } = null!;

    public bool HasPermissionsRequest { get; set; }

    public string LastActionText { get; set; } = null!;

    public bool ShowAgreementId { get; set; } = true;

    public static implicit operator EmployerDetailsViewModel(GetProviderRelationshipResponse response)
    {
        return new EmployerDetailsViewModel
        {
            AccountLegalEntityId = response.AccountLegalEntityId,
            AccountLegalEntityPublicHashedId = response.AccountLegalEntityPublicHashedId,
            AccountLegalEntityName = response.AccountLegalEntityName.ToUpper(),
            Ukprn = response.Ukprn,
            LastAction = response.LastAction,
            LastActionDate = SetLastActionDate(response),
            ProviderName = response.ProviderName.ToUpper(),
            Operations = response.Operations,
            LastRequestOperations = SetLastRequestOperations(response),
            HasPermissionsRequest = SetHasPermissionsRequest(response),
            HasExistingPermissions = SetHasExistingPermissions(response)
        };
    }

    public static implicit operator EmployerDetailsViewModel(GetRequestsByRequestIdResponse response)
    {
        return new EmployerDetailsViewModel
        {
            AccountLegalEntityName = response.EmployerOrganisationName!.ToUpper(),
            Ukprn = response.Ukprn,
            LastActionDate = response.RequestedDate.ToString("d MMM yyyy"),
            ProviderName = response.ProviderName.ToUpper(),
            Operations = Array.Empty<Operation>(),
            LastRequestOperations = response.Operations,
            HasPermissionsRequest = true,
            HasExistingPermissions = false,
            ShowAgreementId = SetShowAgreementId(response)
        };
    }

    private static string SetLastActionDate(GetProviderRelationshipResponse response)
    {
        if (response.LastRequestTime.HasValue)
        {
            return response.LastRequestTime!.Value.Date.ToString("d MMM yyyy");
        }

        return response.LastActionTime is null ? string.Empty : response.LastActionTime!.Value.Date.ToString("d MMM yyyy");
    }

    private static Operation[] SetLastRequestOperations(GetProviderRelationshipResponse response)
    {
        if (response.LastRequestOperations != null && response.LastRequestOperations.Length != 0)
            return response.LastRequestOperations;
        return Array.Empty<Operation>();
    }

    private static bool SetHasPermissionsRequest(GetProviderRelationshipResponse response)
    {
        return response.LastRequestOperations != null && response.LastRequestOperations.Length != 0
            && (response.LastRequestStatus == RequestStatus.Sent || response.LastRequestStatus == RequestStatus.New);
    }

    private static bool SetHasExistingPermissions(GetProviderRelationshipResponse response)
    {
        if (response.LastAction == PermissionAction.RecruitRelationship ||
            response.LastAction == PermissionAction.ApprovalsRelationship)
        {
            return true;
        }

        if (response.Operations.Length == 0 &&
            response.LastRequestOperations != null &&
            response.LastRequestOperations.Length != 0)
        {
            return false;
        }

        return true;
    }

    private static string[] SetPermissionText(Operation[] operations)
    {
        string[] permissionsText = new string[2];

        permissionsText[0] = SetPermissionsText.NoPermissionText;
        if (operations.Contains(Operation.CreateCohort))
            permissionsText[0] = SetPermissionsText.CohortsPermissionText;

        permissionsText[1] = SetPermissionsText.NoPermissionText;
        if (operations.Contains(Operation.Recruitment))
            permissionsText[1] = SetPermissionsText.RecruitmentPermissionText;
        if (operations.Contains(Operation.RecruitmentRequiresReview))
            permissionsText[1] = SetPermissionsText.RecruitmentWithReviewPermissionText;

        return permissionsText;
    }

    private static bool SetShowAgreementId(GetRequestsByRequestIdResponse response)
    {
        return response.AccountLegalEntityId != null;
    }
}

