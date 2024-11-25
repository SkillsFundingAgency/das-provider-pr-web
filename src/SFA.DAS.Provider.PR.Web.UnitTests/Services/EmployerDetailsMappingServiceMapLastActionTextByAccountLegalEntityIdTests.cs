using AutoFixture.NUnit3;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Services;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Services;
public class EmployerDetailsMappingServiceMapLastActionTextByAccountLegalEntityIdTests
{
    [Test]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.AccountCreated,
        RequestStatus.Accepted, EmployerDetailsMappingService.CreateOrAddAccountRequestAcceptedText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.AccountAdded,
        RequestStatus.Accepted, EmployerDetailsMappingService.PermissionSetText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.AccountCreated,
        RequestStatus.Accepted, EmployerDetailsMappingService.PermissionSetText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.AccountCreated,
        RequestStatus.Declined, EmployerDetailsMappingService.UpdatePermissionRequestDeclinedText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.AccountCreated,
        RequestStatus.Expired, EmployerDetailsMappingService.UpdatePermissionRequestExpiredText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.PermissionUpdated,
        RequestStatus.Accepted, EmployerDetailsMappingService.PermissionSetText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.RecruitRelationship,
        RequestStatus.Accepted, EmployerDetailsMappingService.RelationshipByRecruitText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.ApprovalsRelationship,
        RequestStatus.Accepted, EmployerDetailsMappingService.RelationshipByApprovalText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.AccountCreated,
        RequestStatus.New, "")]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.PermissionUpdated,
        RequestStatus.New, "")]
    public void MapTextByAccountLegalEntityId_LastActionTextIsSet(string requestType, PermissionAction lastAction,
        RequestStatus lastRequestStatus, string expected, GetProviderRelationshipResponse response)
    {
        response.LastRequestType = requestType;
        response.LastAction = lastAction;
        response.LastRequestStatus = lastRequestStatus;

        var result = EmployerDetailsMappingService.MapLastActionTextByAccountLegalEntityId(response);

        Assert.That(result, Is.EqualTo(expected));
    }
}
