using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Web.Models;
public class EmployerDetailsViewModel
{
    public const string NoPermissionText = "No";
    public const string RecruitmentPermissionText = "Yes";
    public const string CohortsPermissionText = "Yes, employer will review records";
    public const string RecruitmentRequiresReviewPermissionText = "Yes, employer will review adverts";

    public const string PendingAddTrainingProviderAndPermissionsRequestText = "Add training provider and permissions request sent";
    public const string AccountCreatedPermissionsSetText = "Apprenticeship service account created";
    public const string PendingPermissionRequestUpdatedText = "Permissions request sent";
    public const string PermissionUpdateAcceptedText = "Permissions set";


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

    public string[] CurrentPermissions => SetPermissionsText(Operations);

    public string[] RequestedPermissions => SetPermissionsText(LastRequestOperations ?? Array.Empty<Operation>());

    public string EmployersLink { get; set; } = null!;

    public bool HasPermissionsRequest { get; set; }

    public string LastActionText { get; set; } = null!;

    public string LastRequestType { get; set; } = null!;


    public static implicit operator EmployerDetailsViewModel(GetProviderRelationshipResponse response)
    {
        return new EmployerDetailsViewModel
        {
            AccountLegalEntityId = response.AccountLegalEntityId,
            AccountLegalEntityPublicHashedId = response.AccountLegalEntityPublicHashedId,
            AccountLegalEntityName = response.AccountLegalEntityName,
            Ukprn = response.Ukprn,
            LastAction = response.LastAction,
            LastActionDate = SetLastActionDate(response),
            ProviderName = response.ProviderName,
            Operations = response.Operations,
            LastRequestOperations = SetLastRequestOperations(response),
            HasPermissionsRequest = SetHasPermissionsRequest(response),
            LastActionText = SetLastActionText(response),
            HasExistingPermissions = SetHasExistingPermissions(response)
        };
    }

    private static string SetLastActionDate(GetProviderRelationshipResponse response)
    {
        if (response.LastActionTime != null)
            return response.LastActionTime.Value.Date.ToShortDateString();
        return "";
    }
    private static Operation[] SetLastRequestOperations(GetProviderRelationshipResponse response)
    {
        if (response.LastRequestOperations != null && response.LastRequestOperations.Length != 0)
            return response.LastRequestOperations;
        return Array.Empty<Operation>();
    }

    private static bool SetHasPermissionsRequest(GetProviderRelationshipResponse response)
    {
        return response.LastRequestOperations != null && response.LastRequestOperations.Length != 0;
    }

    private static string SetLastActionText(GetProviderRelationshipResponse response)
    {
        string lastActionText = "";
        if (String.Equals(response.LastRequestType, "CreateAccount", StringComparison.CurrentCultureIgnoreCase)
            && response.LastRequestStatus == RequestStatus.Accepted)
            lastActionText = AccountCreatedPermissionsSetText;
        if (String.Equals(response.LastRequestType, "Permission", StringComparison.CurrentCultureIgnoreCase))
        {
            switch (response.LastRequestStatus)
            {
                case RequestStatus.Sent:
                    switch (response.LastAction)
                    {
                        case PermissionAction.PermissionCreated:
                            lastActionText = PendingAddTrainingProviderAndPermissionsRequestText;
                            break;
                        case PermissionAction.PermissionUpdated:
                            lastActionText = PendingPermissionRequestUpdatedText;
                            break;
                    }
                    break;
                case RequestStatus.Accepted:
                    lastActionText = PermissionUpdateAcceptedText;
                    break;
            }
        }

        return lastActionText;
    }

    private static bool SetHasExistingPermissions(GetProviderRelationshipResponse response)
    {
        if (response.Operations.Length == 0 && response.LastRequestOperations != null && response.LastRequestOperations.Length != 0)
            return false;
        return true;
    }

    private static string[] SetPermissionsText(Operation[] operations)
    {
        string[] permissionsText = new string[2];

        permissionsText[0] = NoPermissionText;
        if (operations.Contains(Operation.CreateCohort))
            permissionsText[0] = CohortsPermissionText;

        permissionsText[1] = NoPermissionText;
        if (operations.Contains(Operation.Recruitment))
            permissionsText[1] = RecruitmentPermissionText;
        if (operations.Contains(Operation.RecruitmentRequiresReview))
            permissionsText[1] = RecruitmentRequiresReviewPermissionText;

        return permissionsText;
    }
}
