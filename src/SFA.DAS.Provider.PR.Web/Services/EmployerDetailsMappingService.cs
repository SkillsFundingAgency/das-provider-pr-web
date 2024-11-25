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

    public const string RequestTypeAddAccount = "AddAccount";
    public const string RequestTypeCreateAccount = "CreateAccount";
    public const string RequestTypePermission = "Permission";

    public static string MapLastActionTextByAccountLegalEntityId(GetProviderRelationshipResponse response)
    {
        if (response.LastRequestType != RequestTypePermission && response.LastAction == PermissionAction.AccountCreated)
            return CreateOrAddAccountRequestAcceptedText;
        if (response.LastRequestType != RequestTypePermission && response.LastAction == PermissionAction.AccountAdded)
            return PermissionSetText;

        if (response.LastRequestType == RequestTypePermission)
        {
            return response.LastRequestStatus switch
            {
                RequestStatus.Accepted => PermissionSetText,
                RequestStatus.Declined => UpdatePermissionRequestDeclinedText,
                RequestStatus.Expired => UpdatePermissionRequestExpiredText,
                _ => string.Empty
            };
        }

        var existingRelationshipStatuses = new List<PermissionAction>() { PermissionAction.RecruitRelationship, PermissionAction.ApprovalsRelationship };

        if (response.LastRequestType == RequestTypeAddAccount && response.LastRequestStatus == RequestStatus.Accepted && !existingRelationshipStatuses.Contains(response.LastAction!.Value))
        {
            return PermissionSetText;
        }

        if (response.LastAction is not null && existingRelationshipStatuses.Contains(response.LastAction.Value))
        {
            return response.LastAction switch
            {
                PermissionAction.RecruitRelationship => RelationshipByRecruitText,
                PermissionAction.ApprovalsRelationship => RelationshipByApprovalText
            };
        }

        return string.Empty;
    }

    public static string MapLastActionTextByRequestId(GetRequestsByRequestIdResponse response)
    {
        return response.RequestType switch
        {
            RequestTypeAddAccount => PendingAddTrainingProviderAndPermissionsRequestText,
            RequestTypeCreateAccount => PendingCreateAccountInvitationText,
            RequestTypePermission => response.Operations.Length == 0
                ? PendingAddTrainingProviderAndPermissionsRequestText
                : UpdatePermissionRequestSentText,
            _ => string.Empty
        };
    }
}
