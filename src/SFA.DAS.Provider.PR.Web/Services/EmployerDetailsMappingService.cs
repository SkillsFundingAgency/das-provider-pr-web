using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Web.Services;

public static class EmployerDetailsMappingService
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

    public static string MapLastActionTextByAccountLegalEntityId(GetProviderRelationshipResponse response)
    {
        if (response.LastAction == PermissionAction.AccountAdded && !string.Equals(response.LastRequestType, "Permission", StringComparison.CurrentCultureIgnoreCase))
        {
            return CreateOrAddAccountRequestAcceptedText;
        }

        if (response.LastAction == PermissionAction.AccountAdded && !string.Equals(response.LastRequestType, "Permission", StringComparison.CurrentCultureIgnoreCase))
        {
            return PermissionSetText;
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

    public static string MapLastActionTextByRequestId(GetRequestsByRequestIdResponse response)
    {
        if (String.Equals(response.RequestType, "AddAccount", StringComparison.CurrentCultureIgnoreCase))
            return PendingAddTrainingProviderAndPermissionsRequestText;
        if (String.Equals(response.RequestType, "CreateAccount", StringComparison.CurrentCultureIgnoreCase))
            return PendingCreateAccountInvitationText;
        return string.Empty;
    }
}
