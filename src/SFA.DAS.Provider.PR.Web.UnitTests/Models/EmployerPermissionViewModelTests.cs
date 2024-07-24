using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;

public class EmployerPermissionViewModelTests
{
    [Test, AutoData]
    public void Operator_SetsName(ProviderRelationshipModel model)
    {
        EmployerPermissionViewModel sut = model;

        sut.Name.Should().Be(model.EmployerName);
    }

    [Test]
    [InlineAutoData(true)]
    [InlineAutoData(false)]
    public void Operator_SetsAgreementId(bool setAgreementId, ProviderRelationshipModel model)
    {
        model.AgreementId = setAgreementId ? Guid.NewGuid().ToString() : null;
        EmployerPermissionViewModel sut = model;

        sut.AgreementId.Should().Be(model.AgreementId);
        sut.HasAgreementId.Should().Be(setAgreementId);
    }

    [Test, AutoData]
    public void Operator_SetsAccountLegalEntityId(ProviderRelationshipModel model)
    {
        EmployerPermissionViewModel sut = model;

        sut.AccountLegalEntityId.Should().Be(model.AccountLegalEntityId);
    }

    [Test]
    [InlineAutoData(true)]
    [InlineAutoData(false)]
    public void Operator_SetsRequestId(bool setRequestId, ProviderRelationshipModel model)
    {
        model.RequestId = setRequestId ? Guid.NewGuid() : null;
        EmployerPermissionViewModel sut = model;

        sut.RequestId.Should().Be(model.RequestId);
        sut.HasPendingRequest.Should().Be(setRequestId);
    }

    [Test]
    [InlineAutoData(true, EmployerPermissionViewModel.CohortsPermissionText)]
    [InlineAutoData(false, EmployerPermissionViewModel.NoPermissionText)]
    public void Operator_SetsCohortPermission(bool hasCohortPermission, string permissionText, ProviderRelationshipModel model)
    {
        model.HasCreateCohortPermission = hasCohortPermission;

        EmployerPermissionViewModel sut = model;

        sut.CohortPermission.Should().Be(permissionText);
    }

    [Test]
    [InlineAutoData(true, false, EmployerPermissionViewModel.RecruitmentPermissionText)]
    [InlineAutoData(false, true, EmployerPermissionViewModel.RecruitmentWithReviewPermissionText)]
    [InlineAutoData(false, false, EmployerPermissionViewModel.NoPermissionText)]
    /// true, true is not a valid scenario, only one of them should be true
    public void Operator_SetsRecruitmentPermission(bool hasRecruitmentPermission, bool hasRecruitmentWithReviewPermission, string permissionText, ProviderRelationshipModel model)
    {
        model.HasRecruitmentPermission = hasRecruitmentPermission;
        model.HasRecruitmentWithReviewPermission = hasRecruitmentWithReviewPermission;

        EmployerPermissionViewModel sut = model;

        sut.RecruitmentPermision.Should().Be(permissionText);
    }
}
