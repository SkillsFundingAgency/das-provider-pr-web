using AutoFixture.NUnit3;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;
public class EmployerDetailsModelTests
{
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
            Assert.That(actual.LastActionDate, Is.EqualTo(response.LastActionTime.Value.Date.ToShortDateString()));
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
    [InlineAutoData(RequestStatus.Sent, PermissionAction.PermissionCreated, EmployerDetailsViewModel.PendingAddTrainingProviderAndPermissionsRequestText)]
    [InlineAutoData(RequestStatus.Sent, PermissionAction.ApprovalsRelationship, EmployerDetailsViewModel.PendingNotImplementedText)]
    [InlineAutoData(RequestStatus.New, PermissionAction.PermissionCreated, EmployerDetailsViewModel.NotPendingNotImplementedText)]
    public void LastActionTextIsSetCorrectly(RequestStatus status, PermissionAction action, string expected,
        GetProviderRelationshipResponse response)
    {
        response.LastRequestStatus = status;
        response.LastAction = action;

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.LastActionText, Is.EqualTo(expected));
    }

    [Test, AutoData]
    public void OperationsContainsCreateCohort_CurrentPermissionsIncludeCreateCohortText(
        GetProviderRelationshipResponse response)
    {
        response.Operations = new Operation[] { Operation.CreateCohort };

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.CurrentPermissions.Contains(EmployerDetailsViewModel.CohortsPermissionText));
    }

    [Test, AutoData]
    public void OperationsContainsRecruitment_CurrentPermissionsIncludeRecruitmentText(
        GetProviderRelationshipResponse response)
    {
        response.Operations = new Operation[] { Operation.Recruitment };

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.CurrentPermissions.Contains(EmployerDetailsViewModel.RecruitmentPermissionText));
    }

    [Test, AutoData]
    public void OperationsContainsRecruitmentRequiresReview_CurrentPermissionsIncludeRecruitmentRequiresReviewText(
        GetProviderRelationshipResponse response)
    {
        response.Operations = new Operation[] { Operation.RecruitmentRequiresReview };

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.CurrentPermissions.Contains(EmployerDetailsViewModel.RecruitmentRequiresReviewPermissionText));
    }
}
