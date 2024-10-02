﻿using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Web.Models;
public class EmployerDetailsViewModel
{
    public const string NoPermissionText = "No";
    public const string RecruitmentPermissionText = "Yes";
    public const string CohortsPermissionText = "Yes, employer will review records";
    public const string RecruitmentRequiresReviewPermissionText = "Yes, employer will review adverts";

    public const string PendingAddTrainingProviderAndPermissionsRequestText = "Add training provider and permissions request sent";
    //The following are WIP, to be replaced in later stories as more variations of the employer details page are created
    public const string PendingNotImplementedText = "PENDING REQUEST - NOT YET IMPLEMENTED";
    public const string NotPendingNotImplementedText = "NO PENDING REQUEST - NOT YET IMPLEMENTED";


    public long AccountLegalEntityId { get; set; }

    public string AccountLegalEntityPublicHashedId { get; set; } = null!;

    public string AccountLegalEntityName { get; set; } = null!;

    public long Ukprn { get; set; }

    public PermissionAction? LastAction { get; set; }

    public string LastActionDate { get; set; } = null!;

    public string ProviderName { get; set; } = null!;

    public bool PendingPermissionRequest { get; set; } = true;

    public Operation[] Operations { get; set; } = [];

    public Operation[]? LastRequestOperations { get; set; } = [];

    public string[] CurrentPermissions => SetPermissionsText(Operations);

    public string[] RequestedPermissions => SetPermissionsText(LastRequestOperations ?? Array.Empty<Operation>());

    public string EmployersLink { get; set; } = null!;

    public bool HasPermissionsRequest { get; set; }

    public string LastActionText { get; set; } = null!;


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
            LastActionText = SetLastActionText(response)
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
        if (response.LastRequestStatus == RequestStatus.Sent)
        {
            switch (response.LastAction)
            {
                case PermissionAction.PermissionCreated:
                    return PendingAddTrainingProviderAndPermissionsRequestText;
                default:
                    return PendingNotImplementedText;
            }
        }

        return NotPendingNotImplementedText;
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