﻿using AutoFixture.NUnit3;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;
public class EmployerDetailsViewModelTests
{
    [Test, AutoData]
    public void ModelIsCreatedCorrectly_FromGetProviderRelationshipResponseObject(GetProviderRelationshipResponse response)
    {
        response.LastRequestOperations = Array.Empty<Operation>();
        response.LastRequestStatus = RequestStatus.Declined;
        response.LastAction = PermissionAction.RecruitRelationship;

        var actual = (EmployerDetailsViewModel)response;

        Assert.Multiple(() =>
        {
            Assert.That(actual.AccountLegalEntityId, Is.EqualTo(response.AccountLegalEntityId));
            Assert.That(actual.AccountLegalEntityPublicHashedId, Is.EqualTo(response.AccountLegalEntityPublicHashedId));
            Assert.That(actual.AccountLegalEntityName, Is.EqualTo(response.AccountLegalEntityName.ToUpper()));
            Assert.That(actual.Ukprn, Is.EqualTo(response.Ukprn));
            Assert.That(actual.LastAction, Is.EqualTo(response.LastAction));
            Assert.That(actual.LastActionDate, Is.EqualTo(response.LastRequestTime?.ToString("d MMM yyyy")));
            Assert.That(actual.ProviderName, Is.EqualTo(response.ProviderName.ToUpper()));
            Assert.That(actual.Operations, Is.EqualTo(response.Operations));
            Assert.That(actual.LastRequestOperations, Is.EqualTo(Array.Empty<Operation>()));
            Assert.That(actual.HasPermissionsRequest, Is.False);
        });
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_LastActionText_ExistingRecruitRelationshipText(GetProviderRelationshipResponse response)
    {
        response.LastRequestOperations = Array.Empty<Operation>();
        response.LastRequestStatus = RequestStatus.Declined;
        response.LastAction = PermissionAction.RecruitRelationship;

        var actual = (EmployerDetailsViewModel)response;
        Assert.That(actual.LastActionText, Is.EqualTo(EmployerDetailsViewModel.RelationshipByRecruitText));
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_LastActionText_ExistingApprovalsRelationshipText(GetProviderRelationshipResponse response)
    {
        response.LastRequestOperations = Array.Empty<Operation>();
        response.LastRequestStatus = RequestStatus.Declined;
        response.LastAction = PermissionAction.ApprovalsRelationship;

        var actual = (EmployerDetailsViewModel)response;
        Assert.That(actual.LastActionText, Is.EqualTo(EmployerDetailsViewModel.RelationshipByApprovalText));
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_SetLastActionDate_LastRequestTimeHasValue(GetProviderRelationshipResponse response)
    {
        response.LastRequestTime = DateTime.UtcNow;
        var actual = (EmployerDetailsViewModel)response;
        Assert.That(actual.LastActionDate, Is.EqualTo(DateTime.UtcNow.Date.ToString("d MMM yyyy")));
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_SetLastActionDate_LastRequestTimeNull_ReturnsLastActionTime(GetProviderRelationshipResponse response)
    {
        response.LastRequestTime = null;
        response.LastActionTime = DateTime.UtcNow.AddDays(-1);

        var actual = (EmployerDetailsViewModel)response;
        Assert.That(actual.LastActionDate, Is.EqualTo(DateTime.UtcNow.AddDays(-1).Date.ToString("d MMM yyyy")));
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_SetLastActionDate_LastActionTimeNull_ReturnsEmptyString(GetProviderRelationshipResponse response)
    {
        response.LastRequestTime = null;
        response.LastActionTime = null;

        var actual = (EmployerDetailsViewModel)response;
        Assert.That(actual.LastActionDate, Is.EqualTo(string.Empty));
    }

    [Test, AutoData]
    public void ModelIsCreatedCorrectly_FromGetRequestsByRequestIdResponseObject(GetRequestsByRequestIdResponse response)
    {
        var actual = (EmployerDetailsViewModel)response;

        Assert.Multiple(() =>
        {
            Assert.That(actual.AccountLegalEntityName, Is.EqualTo(response.EmployerOrganisationName!.ToUpper()));
            Assert.That(actual.Ukprn, Is.EqualTo(response.Ukprn));
            Assert.That(actual.LastActionDate, Is.EqualTo(response.RequestedDate.ToString("d MMM yyyy")));
            Assert.That(actual.ProviderName, Is.EqualTo(response.ProviderName.ToUpper()));
            Assert.That(actual.Operations, Is.EqualTo(Array.Empty<Operation>()));
            Assert.That(actual.LastRequestOperations, Is.EqualTo(response.Operations));
            Assert.That(actual.HasPermissionsRequest, Is.True);
            Assert.That(actual.HasExistingPermissions, Is.False);
            Assert.That(actual.ShowAgreementId, Is.True);
        });
    }

    [Test, AutoData]
    public void FromGetRequestsByRequestIdResponseOperator_HasAccountLegalEntityId_ShowAgreementIsTrue(GetRequestsByRequestIdResponse response)
    {
        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.ShowAgreementId, Is.True);
    }

    [Test, AutoData]
    public void FromGetRequestsByRequestIdResponseOperator_DoesNotHaveAccountLegalEntityId_ShowAgreementIdIsFalse(GetRequestsByRequestIdResponse response)
    {
        response.AccountLegalEntityId = null;
        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.ShowAgreementId, Is.False);
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_SetHasExistingPermissions_ApprovalsRelationshipReturnsTrue(GetProviderRelationshipResponse response)
    {
        response.LastAction = PermissionAction.ApprovalsRelationship;
        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.HasPermissionsRequest, Is.True);
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_SetHasExistingPermissions_RecruitRelationshipReturnsTrue(GetProviderRelationshipResponse response)
    {
        response.LastAction = PermissionAction.RecruitRelationship;
        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.HasPermissionsRequest, Is.True);
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_SetHasExistingPermissions_HasExistingOperations_ReturnsTrue(GetProviderRelationshipResponse response)
    {
        response.LastAction = PermissionAction.PermissionUpdated;
        response.Operations = [Operation.Recruitment];
        response.LastRequestOperations = [Operation.Recruitment];

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.HasPermissionsRequest, Is.True);
    }

    [Test, AutoData]
    public void EmployerDetailsViewModel_SetHasExistingPermissions_HasNoExistingOperations_ReturnsFalse(GetProviderRelationshipResponse response)
    {
        response.LastAction = PermissionAction.PermissionUpdated;
        response.Operations = [];
        response.LastRequestOperations = [];

        var actual = (EmployerDetailsViewModel)response;

        Assert.That(actual.HasPermissionsRequest, Is.False);
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

        Assert.That(actual.CurrentPermissions, Does.Contain(expected));
    }

    [Test]
    [InlineAutoData(new Operation[] { }, new[] { Operation.CreateCohort }, false)]
    [InlineAutoData(new[] { Operation.CreateCohort }, new Operation[] { }, true)]
    [InlineAutoData(new[] { Operation.CreateCohort }, new[] { Operation.CreateCohort }, true)]
    public void HasExistingPermissionsSetCorrectly(Operation[] operations, Operation[] lastRequestOperations,
        bool expected, GetProviderRelationshipResponse response)
    {
        response.Operations = operations;
        response.LastAction = PermissionAction.PermissionUpdated;
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


    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("action text", true)]
    public void EmployerDetailsViewModel_SetLastActionText_SetShowLastActionText(string? lastActionText, bool expectedShowLastActionText)
    {
        var actual = new EmployerDetailsViewModel { LastActionText = lastActionText! };
        Assert.That(actual.ShowLastActionText, Is.EqualTo(expectedShowLastActionText));
    }
}
