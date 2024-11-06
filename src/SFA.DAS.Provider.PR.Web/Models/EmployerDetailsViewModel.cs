
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;

namespace SFA.DAS.Provider.PR.Web.Models;
public class EmployerDetailsViewModel
{
    public const string PendingAddTrainingProviderAndPermissionsRequestText = "Add training provider and permissions request sent";
    public const string PendingCreateAccountInvitationText = "Create apprenticeship service account invitation sent";
    public const string AccountCreatedPermissionsSetText = "Apprenticeship service account created";
    public const string PendingPermissionRequestUpdatedText = "Permissions request sent";
    public const string PermissionUpdateAcceptedText = "Permissions set";
    public const string PermissionUpdateDeclinedText = "Permissions request declined";
    public const string PermissionUpdateExpiredText = "Permissions request expired";
    public const string ExistingRecruitRelationshipText = "Added you as training provider for new apprentice vacancy";
    public const string ExistingApprovalsRelationshipText = "Added you as training provider for new apprentice";


    public long AccountLegalEntityId { get; set; }

    public string AccountLegalEntityPublicHashedId { get; set; } = null!;

    public string AccountLegalEntityName { get; set; } = null!;

    public long Ukprn { get; set; }

    public PermissionAction? LastAction { get; set; }

    public string LastActionDate { get; set; } = null!;

    public string ProviderName { get; set; } = null!;

    public bool HasExistingPermissions { get; set; } = true;

    public Operation[] Operations { get; set; } = [];

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
            LastActionText = SetLastActionText(response),
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
            LastActionText = SetLastActionText(response),
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

    private static string SetLastActionText(GetProviderRelationshipResponse response)
    {
        string lastActionText = "";

        if (String.Equals(response.LastRequestType, "CreateAccount", StringComparison.CurrentCultureIgnoreCase)
            && response.LastRequestStatus == RequestStatus.Accepted)
            lastActionText = AccountCreatedPermissionsSetText;

        if (String.Equals(response.LastRequestType, "Permission", StringComparison.CurrentCultureIgnoreCase))
        {
            if (response.LastRequestStatus == RequestStatus.Sent || response.LastRequestStatus == RequestStatus.New)
            {
                if (response.LastAction == PermissionAction.PermissionCreated)
                    lastActionText = PendingAddTrainingProviderAndPermissionsRequestText;
                else
                {
                    lastActionText = PendingPermissionRequestUpdatedText;
                }
            }
            switch (response.LastRequestStatus)
            {
                case RequestStatus.Accepted:
                    lastActionText = PermissionUpdateAcceptedText;
                    break;
                case RequestStatus.Declined:
                    lastActionText = PermissionUpdateDeclinedText;
                    break;
                case RequestStatus.Expired:
                    lastActionText = PermissionUpdateExpiredText;
                    break;
            }
        }
        var statuses = new List<PermissionAction>() { PermissionAction.RecruitRelationship, PermissionAction.ApprovalsRelationship };
        if(response.LastAction is not null && statuses.Contains(response.LastAction.Value))
        {
            switch (response.LastAction)
            {
                case PermissionAction.RecruitRelationship:
                    lastActionText = ExistingRecruitRelationshipText;
                    break;
                case PermissionAction.ApprovalsRelationship:
                    lastActionText = ExistingApprovalsRelationshipText;
                    break;
            }
        }

        return lastActionText;
    }

    private static string SetLastActionText(GetRequestsByRequestIdResponse response)
    {
        string lastActionText = "";

        if (String.Equals(response.RequestType, "AddAccount", StringComparison.CurrentCultureIgnoreCase))
            lastActionText = PendingAddTrainingProviderAndPermissionsRequestText;
        if (String.Equals(response.RequestType, "CreateAccount", StringComparison.CurrentCultureIgnoreCase))
            lastActionText = PendingCreateAccountInvitationText;
        return lastActionText;
    }

    private static bool SetHasExistingPermissions(GetProviderRelationshipResponse response)
    {
        if (response.Operations.Length == 0 && response.LastRequestOperations != null && response.LastRequestOperations.Length != 0)
            return false;
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
