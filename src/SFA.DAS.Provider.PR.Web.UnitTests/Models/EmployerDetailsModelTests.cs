using AutoFixture.NUnit3;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;
public class EmployerDetailsModelTests
{
    public const string CreateAccountRequestType = "CreateAccount";
    public const string PermissionRequestType = "Permission";

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
            Assert.That(actual.LastActionText, Is.EqualTo("NO PENDING REQUEST - NOT YET IMPLEMENTED"));
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

    [Test, AutoData]
    public void ResponseContainsLastRequestOperations_ModelBuildsHasPermissionsRequestCorrectly(
        GetProviderRelationshipResponse response)
    {
        response.LastRequestOperations = new Operation[2];

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.HasPermissionsRequest, Is.True);
    }

    [Test]
    [InlineAutoData(RequestStatus.Sent, PermissionAction.PermissionCreated, PermissionRequestType, EmployerDetailsViewModel.PendingAddTrainingProviderAndPermissionsRequestText)]
    [InlineAutoData(RequestStatus.Sent, PermissionAction.PermissionUpdated, PermissionRequestType, EmployerDetailsViewModel.PendingPermissionRequestUpdatedText)]
    [InlineAutoData(RequestStatus.Accepted, PermissionAction.PermissionUpdated, PermissionRequestType, EmployerDetailsViewModel.PermissionUpdateAcceptedText)]
    [InlineAutoData(RequestStatus.Declined, PermissionAction.PermissionUpdated, PermissionRequestType, EmployerDetailsViewModel.PermissionUpdateDeclinedText)]
    [InlineAutoData(RequestStatus.Expired, PermissionAction.PermissionUpdated, PermissionRequestType, EmployerDetailsViewModel.PermissionUpdateExpiredText)]
    [InlineAutoData(RequestStatus.Accepted, PermissionAction.PermissionCreated, CreateAccountRequestType, EmployerDetailsViewModel.AccountCreatedPermissionsSetText)]
    public void LastActionTextIsSetCorrectly(RequestStatus status, PermissionAction action, string lastRequestType, string expected,
        GetProviderRelationshipResponse response)
    {
        response.LastRequestStatus = status;
        response.LastAction = action;
        response.LastRequestType = lastRequestType;

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.LastActionText, Is.EqualTo(expected));
    }

    [Test]
    [InlineAutoData(new Operation[] { Operation.CreateCohort }, EmployerDetailsViewModel.CohortsPermissionText)]
    [InlineAutoData(new Operation[] { Operation.Recruitment }, EmployerDetailsViewModel.RecruitmentPermissionText)]
    [InlineAutoData(new Operation[] { Operation.RecruitmentRequiresReview }, EmployerDetailsViewModel.RecruitmentRequiresReviewPermissionText)]
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
}
