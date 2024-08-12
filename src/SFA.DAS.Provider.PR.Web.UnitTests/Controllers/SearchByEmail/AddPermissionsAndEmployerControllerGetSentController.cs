using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByEmail;
public class AddPermissionsAndEmployerControllerGetSentController
{
    private static readonly string HomeLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModelFromSessionModel(int ukprn, long accountId, long accountLegalEntityId, string accountLegalName)
    {
        var email = "test@test.com";
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = email,
            AccountLegalEntityId = accountLegalEntityId,
            AccountLegalEntityName = accountLegalName,
            AccountId = accountId,
            PermissionToAddCohorts = SetPermissions.AddRecords.Yes,
            PermissionToRecruit = SetPermissions.RecruitApprentices.No
        });
        AddPermissionsAndEmployerController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<AddPermissionsAndEmployerSubmitViewModel>>());

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.Home, HomeLink);

        var result = sut.AddEmployerAndPermissionsSent(ukprn);

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
    public void Get_SessionModelInvalid_RedirectsToAddEmployerStart(string? email, long? accountLegalEntityId, string? permissionToAddCohorts, int ukprn)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(
            new AddEmployerSessionModel
            {
                Email = email!,
                AccountLegalEntityId = accountLegalEntityId,
                AccountLegalEntityName = "name",
                PermissionToAddCohorts = permissionToAddCohorts,
                PermissionToRecruit = SetPermissions.RecruitApprentices.No
            });

        AddPermissionsAndEmployerController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<AddPermissionsAndEmployerSubmitViewModel>>());

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.AddEmployerAndPermissionsSent(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqInlineAutoData]
    public void Get_SessionModelNull_RedirectsToAddEmployerStart(int ukprn)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        AddPermissionsAndEmployerController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<AddPermissionsAndEmployerSubmitViewModel>>());

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.AddEmployerAndPermissionsSent(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }
}
