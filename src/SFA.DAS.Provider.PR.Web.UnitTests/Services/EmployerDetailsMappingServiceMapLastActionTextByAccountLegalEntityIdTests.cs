using AutoFixture.NUnit3;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Services;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Services;
public class EmployerDetailsMappingServiceMapLastActionTextByAccountLegalEntityIdTests
{
    public const string TodayDateTime = "01/01/2020";
    public const string YesterdayDateTime = "31/12/2019";

    [Test]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.AccountCreated,
        RequestStatus.Accepted, TodayDateTime, TodayDateTime,
        EmployerDetailsMappingService.CreateOrAddAccountRequestAcceptedText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.AccountAdded,
        RequestStatus.Accepted, TodayDateTime, TodayDateTime,
        EmployerDetailsMappingService.PermissionSetText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.PermissionUpdated,
        RequestStatus.Sent, TodayDateTime, YesterdayDateTime,
        EmployerDetailsMappingService.PermissionSetText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.AccountCreated,
        RequestStatus.Accepted, TodayDateTime, TodayDateTime,
        EmployerDetailsMappingService.PermissionSetText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.AccountCreated,
        RequestStatus.Declined, TodayDateTime, TodayDateTime,
        EmployerDetailsMappingService.UpdatePermissionRequestDeclinedText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.AccountCreated,
        RequestStatus.Expired, TodayDateTime, TodayDateTime,
        EmployerDetailsMappingService.UpdatePermissionRequestExpiredText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.PermissionUpdated,
        RequestStatus.Accepted, TodayDateTime, TodayDateTime,
        EmployerDetailsMappingService.PermissionSetText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.RecruitRelationship,
        RequestStatus.Accepted, TodayDateTime, TodayDateTime,
        EmployerDetailsMappingService.RelationshipByRecruitText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.ApprovalsRelationship,
        RequestStatus.Accepted, TodayDateTime, TodayDateTime,
        EmployerDetailsMappingService.RelationshipByApprovalText)]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypePermission, PermissionAction.AccountCreated,
        RequestStatus.New, TodayDateTime, TodayDateTime, "")]
    [InlineAutoData(EmployerDetailsMappingService.RequestTypeAddAccount, PermissionAction.PermissionUpdated,
        RequestStatus.New, TodayDateTime, TodayDateTime, "")]
    public void MapTextByAccountLegalEntityId_LastActionTextIsSet(string requestType, PermissionAction lastAction,
        RequestStatus lastRequestStatus, string lastActionTime, string lastRequestTime, string expected, GetProviderRelationshipResponse response)
    {
        response.LastRequestType = requestType;
        response.LastAction = lastAction;
        response.LastRequestStatus = lastRequestStatus;
        response.LastActionTime = DateTime.Parse(lastActionTime);
        response.LastRequestTime = DateTime.Parse(lastRequestTime);

        var result = EmployerDetailsMappingService.MapLastActionText(response);

        Assert.That(result, Is.EqualTo(expected));
    }
}
