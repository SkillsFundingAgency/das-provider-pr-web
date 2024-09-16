using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Requests.Commands;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR.Web.Services;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByEmail;
public class AddPermissionsAndEmployerControllerGetSentControllerTests
{
    private static readonly string HomeLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public async Task Get_BuildsExpectedViewModelFromSessionModel(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<AddPermissionsAndEmployerSubmitModel>> validatorMock,
        [Greedy] AddPermissionsAndEmployerController sut,
        int ukprn,
        long accountId,
        long accountLegalEntityId,
        string accountLegalName,
        CancellationToken cancellationToken)
    {
        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = email,
            AccountLegalEntityId = accountLegalEntityId,
            AccountLegalEntityName = accountLegalName,
            AccountId = accountId,
            PermissionToAddCohorts = SetPermissions.AddRecords.Yes,
            PermissionToRecruit = SetPermissions.RecruitApprentices.No
        });

        sut.AddDefaultContext();
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.Home, HomeLink);

        var result = await sut.AddEmployerAndPermissionsRequested(ukprn, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        AddEmployerAndPermissionsSentViewModel? viewModel = viewResult.Model as AddEmployerAndPermissionsSentViewModel;

        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Delete<AddEmployerSessionModel>(), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(viewResult.ViewName, Is.EqualTo(AddPermissionsAndEmployerController.ViewPathSent));
            Assert.That(viewModel!.ViewEmployersLink, Is.EqualTo(HomeLink));
            Assert.That(viewModel.Email, Is.EqualTo(email));
            Assert.That(viewModel.LegalName, Is.EqualTo(accountLegalName));
            Assert.That(viewModel.Ukprn, Is.EqualTo(ukprn));
        });
    }

    [Test]
    [MoqInlineAutoData(null, null, null)]
    [MoqInlineAutoData("test@test.com", null, null)]
    [MoqInlineAutoData(null, 134L, null)]
    [MoqInlineAutoData(null, null, SetPermissions.AddRecords.Yes)]
    [MoqInlineAutoData("test@test.com", 134L, null)]
    [MoqInlineAutoData("test@test.com", null, SetPermissions.AddRecords.Yes)]
    [MoqInlineAutoData(null, 1345L, SetPermissions.AddRecords.Yes)]
    public async Task Get_SessionModelInvalid_RedirectsToAddEmployerStart(
        string? email,
        long? accountLegalEntityId,
        string? permissionToAddCohorts,
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<AddPermissionsAndEmployerSubmitModel>> validatorMock,
        [Greedy] AddPermissionsAndEmployerController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(
            new AddEmployerSessionModel
            {
                Email = email!,
                AccountLegalEntityId = accountLegalEntityId,
                AccountLegalEntityName = "name",
                PermissionToAddCohorts = permissionToAddCohorts,
                PermissionToRecruit = SetPermissions.RecruitApprentices.No
            });

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = await sut.AddEmployerAndPermissionsRequested(ukprn, new CancellationToken());

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqInlineAutoData]
    public async Task Get_SessionModelNull_RedirectsToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<AddPermissionsAndEmployerSubmitModel>> validatorMock,
        [Greedy] AddPermissionsAndEmployerController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = await sut.AddEmployerAndPermissionsRequested(ukprn, new CancellationToken());

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.No)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.Yes)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.YesWithReview)]
    public async Task Get_PostsExpectedAddRequestForSingleOperation(
        string addRecordsOperation,
        string addRecruitmentOperation,
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<AddPermissionsAndEmployerSubmitModel>> validatorMock,
        [Greedy] AddPermissionsAndEmployerController sut,
        int ukprn,
        long accountId,
        long accountLegalEntityId,
        string accountLegalName,
        CancellationToken cancellationToken)
    {
        var email = "test@test.com";

        var addEmployerSessionModel = new AddEmployerSessionModel
        {
            Email = email,
            AccountLegalEntityId = accountLegalEntityId,
            AccountLegalEntityName = accountLegalName,
            AccountId = accountId,
            PermissionToAddCohorts = addRecordsOperation,
            PermissionToRecruit = addRecruitmentOperation
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        var expectedOperations = OperationsMappingService.MapDescriptionsToOperations(addEmployerSessionModel);

        var requestId = Guid.NewGuid();

        outerApiClientMock.Setup(o => o.AddRequest(It.IsAny<AddAccountRequestCommand>(), cancellationToken)).ReturnsAsync(new AddAccountRequestCommandResponse(requestId));


        sut.AddDefaultContext();
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.Home, HomeLink);

        await sut.AddEmployerAndPermissionsRequested(ukprn, cancellationToken);

        outerApiClientMock.Verify(o => o.AddRequest(It.Is<AddAccountRequestCommand>(
            s => s.EmployerContactEmail == email
            && s.AccountLegalEntityId == accountLegalEntityId
            && s.Ukprn == ukprn
          && s.Operations.First() == expectedOperations.First()
        ), cancellationToken), Times.Once);
    }

    [Test]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.Yes)]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.YesWithReview)]
    public async Task Get_PostsExpectedAddRequestForTwoOperations(
        string addRecordsOperation,
        string addRecruitmentOperation,
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<AddPermissionsAndEmployerSubmitModel>> validatorMock,
        [Greedy] AddPermissionsAndEmployerController sut,
        int ukprn, long accountId,
        long accountLegalEntityId,
        string accountLegalName,
        CancellationToken cancellationToken)
    {
        var email = "test@test.com";

        var addEmployerSessionModel = new AddEmployerSessionModel
        {
            Email = email,
            AccountLegalEntityId = accountLegalEntityId,
            AccountLegalEntityName = accountLegalName,
            AccountId = accountId,
            PermissionToAddCohorts = addRecordsOperation,
            PermissionToRecruit = addRecruitmentOperation
        };

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        var expectedOperations = OperationsMappingService.MapDescriptionsToOperations(addEmployerSessionModel);

        var requestId = Guid.NewGuid();

        outerApiClientMock.Setup(o => o.AddRequest(It.IsAny<AddAccountRequestCommand>(), cancellationToken)).ReturnsAsync(new AddAccountRequestCommandResponse(requestId));

        sut.AddDefaultContext();
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.Home, HomeLink);

        await sut.AddEmployerAndPermissionsRequested(ukprn, cancellationToken);

        outerApiClientMock.Verify(o => o.AddRequest(It.Is<AddAccountRequestCommand>(
            s => s.EmployerContactEmail == email
            && s.AccountLegalEntityId == accountLegalEntityId
            && s.Ukprn == ukprn
          && s.Operations.First() == expectedOperations.First()
        ), cancellationToken), Times.Once);

        outerApiClientMock.Verify(o => o.AddRequest(It.Is<AddAccountRequestCommand>(
            s => s.EmployerContactEmail == email
                 && s.AccountLegalEntityId == accountLegalEntityId
                 && s.Ukprn == ukprn
                 && s.Operations.Skip(1).First() == expectedOperations.Skip(1).First()
        ), cancellationToken), Times.Once);
    }
}
