using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;

namespace SFA.DAS.Provider.PR.Web.Models;
public class EmployerDetailsViewModel
{
    public const string PendingAddTrainingProviderAndPermissionsRequestText = "Add training provider and permissions request sent";
    public const string PendingCreateAccountInvitationText = "Create apprenticeship service account invitation sent";
    public const string CreateOrAddAccountRequestAcceptedText = "Apprenticeship service account created";
    public const string UpdatePermissionRequestSentText = "Permissions request sent";
    public const string PermissionSetText = "Permissions set";
    public const string UpdatePermissionRequestDeclinedText = "Permissions request declined";
    public const string UpdatePermissionRequestExpiredText = "Permissions request expired";
    public const string RelationshipByRecruitText = "Added you as training provider for new apprentice vacancy";
    public const string RelationshipByApprovalText = "Added you as training provider for new apprentice";

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

    private static bool IsLastActionCreateAccountOrAccountAdded(GetProviderRelationshipResponse response)
    {
        return response.LastAction == PermissionAction.AccountAdded || response.LastAction == PermissionAction.AccountCreated;
    }

    private static string SetLastActionText(GetProviderRelationshipResponse response)
    {
        if (IsLastActionCreateAccountOrAccountAdded(response))
        {
            return CreateOrAddAccountRequestAcceptedText;
        }

        if (string.Equals(response.LastRequestType, "Permission", StringComparison.CurrentCultureIgnoreCase))
        {
            if (response.LastRequestStatus == RequestStatus.Sent || response.LastRequestStatus == RequestStatus.New)
            {
                return response.LastAction == PermissionAction.PermissionCreated ?
                    PendingAddTrainingProviderAndPermissionsRequestText :
                    UpdatePermissionRequestSentText;
            }

            return response.LastRequestStatus switch
            {
                RequestStatus.Accepted => PermissionSetText,
                RequestStatus.Declined => UpdatePermissionRequestDeclinedText,
                RequestStatus.Expired => UpdatePermissionRequestExpiredText,
                _ => string.Empty
            };
        }

        var statuses = new List<PermissionAction>() { PermissionAction.RecruitRelationship, PermissionAction.ApprovalsRelationship };

        if (string.Equals(response.LastRequestType, "AddAccount") && response.LastRequestStatus == RequestStatus.Accepted && !statuses.Contains(response.LastAction!.Value))
        {
            return PermissionSetText;
        }

        if (response.LastAction is not null && statuses.Contains(response.LastAction.Value))
        {
            switch (response.LastAction)
            {
                case PermissionAction.RecruitRelationship:
                    {
                        return RelationshipByRecruitText;
                    }
                case PermissionAction.ApprovalsRelationship:
                    {
                        return RelationshipByApprovalText;
                    }
            }
        }

        return string.Empty;
    }

    private static string SetLastActionText(GetRequestsByRequestIdResponse response)
    {
        if (String.Equals(response.RequestType, "AddAccount", StringComparison.CurrentCultureIgnoreCase))
            return PendingAddTrainingProviderAndPermissionsRequestText;
        if (String.Equals(response.RequestType, "CreateAccount", StringComparison.CurrentCultureIgnoreCase))
            return PendingCreateAccountInvitationText;
        return string.Empty;
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

