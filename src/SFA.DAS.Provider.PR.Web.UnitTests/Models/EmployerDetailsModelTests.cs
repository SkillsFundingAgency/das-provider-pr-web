using AutoFixture.NUnit3;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;
public class EmployerDetailsModelTests
{
    public const string CreateAccountRequestType = "CreateAccount";
    public const string PermissionRequestType = "Permission";
    public const string AddAccountRequestType = "AddAccount";

    [Test, AutoData]
    public void ModelIsCreatedCorrectly_FromApiResponseObject(GetProviderRelationshipResponse response)
    {
        response.LastRequestOperations = Array.Empty<Operation>();
        response.LastRequestStatus = RequestStatus.Declined;

        var actual = (EmployerDetailsViewModel)response;

        Assert.Multiple(() =>
        {
            Assert.That(actual.AccountLegalEntityId, Is.EqualTo(response.AccountLegalEntityId));
            Assert.That(actual.AccountLegalEntityPublicHashedId, Is.EqualTo(response.AccountLegalEntityPublicHashedId));
            Assert.That(actual.AccountLegalEntityName, Is.EqualTo(response.AccountLegalEntityName));
            Assert.That(actual.Ukprn, Is.EqualTo(response.Ukprn));
            Assert.That(actual.LastAction, Is.EqualTo(response.LastAction));
            Assert.That(actual.LastActionDate, Is.EqualTo(response.LastActionTime?.Date.ToShortDateString()));
            Assert.That(actual.ProviderName, Is.EqualTo(response.ProviderName));
            Assert.That(actual.Operations, Is.EqualTo(response.Operations));
            Assert.That(actual.LastRequestOperations, Is.EqualTo(Array.Empty<Operation>()));
            Assert.That(actual.HasPermissionsRequest, Is.False);
        });
    }

    [Test, AutoData]
    public void ResponseContainsLastRequestOperations_ModelBuildsLastRequestOperationsCorrectly(
        GetProviderRelationshipResponse response)
    {
        response.LastRequestOperations = new Operation[2];

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.LastRequestOperations, Is.EqualTo(response.LastRequestOperations));
    }

    [Test]
    [InlineAutoData(RequestStatus.Sent, PermissionAction.PermissionUpdated, PermissionRequestType, EmployerDetailsViewModel.PendingPermissionRequestUpdatedText)]
    [InlineAutoData(RequestStatus.Accepted, PermissionAction.PermissionUpdated, PermissionRequestType, EmployerDetailsViewModel.PermissionUpdateAcceptedText)]
    [InlineAutoData(RequestStatus.Declined, PermissionAction.PermissionUpdated, PermissionRequestType, EmployerDetailsViewModel.PermissionUpdateDeclinedText)]
    [InlineAutoData(RequestStatus.Expired, PermissionAction.PermissionUpdated, PermissionRequestType, EmployerDetailsViewModel.PermissionUpdateExpiredText)]
    [InlineAutoData(RequestStatus.Accepted, PermissionAction.PermissionCreated, CreateAccountRequestType, EmployerDetailsViewModel.AccountCreatedPermissionsSetText)]
    [InlineAutoData(RequestStatus.Accepted, PermissionAction.RecruitRelationship, AddAccountRequestType, EmployerDetailsViewModel.ExistingRecruitRelationshipText)]
    [InlineAutoData(RequestStatus.Accepted, PermissionAction.ApprovalsRelationship, AddAccountRequestType, EmployerDetailsViewModel.ExistingApprovalsRelationshipText)]

    public void LastActionTextIsSetCorrectly_WhenExistingRelationshipExists(RequestStatus status, PermissionAction action, string lastRequestType, string expected,
        GetProviderRelationshipResponse response)
    {
        response.LastRequestStatus = status;
        response.LastAction = action;
        response.LastRequestType = lastRequestType;

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.LastActionText, Is.EqualTo(expected));
    }

    [Test]
    [InlineAutoData("AddAccount", EmployerDetailsViewModel.PendingAddTrainingProviderAndPermissionsRequestText)]
    [InlineAutoData("CreateAccount", EmployerDetailsViewModel.PendingCreateAccountInvitationText)]
    public void LastActionTextIsSetCorrectly_WhenExistingDoesNotRelationshipExist(string lastRequestType, string expected,
        GetRequestsByRequestIdResponse response)
    {
        response.RequestType = lastRequestType;

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.LastActionText, Is.EqualTo(expected));
    }

    [Test]
    [InlineAutoData(new Operation[] { Operation.CreateCohort }, SetPermissionsText.CohortsPermissionText)]
    [InlineAutoData(new Operation[] { Operation.Recruitment }, SetPermissionsText.RecruitmentPermissionText)]
    [InlineAutoData(new Operation[] { Operation.RecruitmentRequiresReview }, SetPermissionsText.RecruitmentWithReviewPermissionText)]
    public void CurrentPermissionsSetCorrectly(Operation[] operations, string expected,
        GetProviderRelationshipResponse response)
    {
        response.Operations = operations;

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.CurrentPermissions.Contains(expected));
    }

    [Test]
    [InlineAutoData(new Operation[] { }, new Operation[] { Operation.CreateCohort }, false)]
    [InlineAutoData(new Operation[] { Operation.CreateCohort }, new Operation[] { }, true)]
    [InlineAutoData(new Operation[] { Operation.CreateCohort }, new Operation[] { Operation.CreateCohort }, true)]
    public void HasExistingPermissionsSetCorrectly(Operation[] operations, Operation[] lastRequestOperations,
        bool expected, GetProviderRelationshipResponse response)
    {
        response.Operations = operations;
        response.LastRequestOperations = lastRequestOperations;

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.HasExistingPermissions, Is.EqualTo(expected));
    }

    [Test]
    [InlineAutoData(new Operation[] { }, RequestStatus.Declined, false)]
    [InlineAutoData(new Operation[] { }, RequestStatus.New, false)]
    [InlineAutoData(new Operation[] { Operation.CreateCohort }, RequestStatus.Declined, false)]
    [InlineAutoData(new Operation[] { Operation.CreateCohort }, RequestStatus.New, true)]
    [InlineAutoData(new Operation[] { Operation.CreateCohort }, RequestStatus.Sent, true)]
    public void HasPermissionsRequestSetCorrectly(Operation[] operations, RequestStatus status, bool expected,
        GetProviderRelationshipResponse response)
    {
        response.LastRequestOperations = operations;
        response.LastRequestStatus = status;

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.HasPermissionsRequest, Is.EqualTo(expected));
    }
}
