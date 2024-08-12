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
public class AddPermissionsAndEmployerControllerGetTests
{
    private static readonly string CancelLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModelFromSessionModel(int ukprn, long accountId, long accountLegalEntityId, string accountLegalName)
    {
        var email = "test@test.com";
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, AccountLegalEntityId = accountLegalEntityId, AccountLegalEntityName = accountLegalName, AccountId = accountId });
        AddPermissionsAndEmployerController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<AddPermissionsAndEmployerSubmitViewModel>>());

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        AddPermissionsAndEmployerViewModel? viewModel = viewResult.Model as AddPermissionsAndEmployerViewModel;

        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(viewResult.ViewName, Is.EqualTo(AddPermissionsAndEmployerController.ViewPath));
            Assert.That(viewModel!.CancelLink, Is.EqualTo(CancelLink));
            Assert.That(viewModel.Email, Is.EqualTo(email));
            Assert.That(viewModel.LegalName, Is.EqualTo(accountLegalName));
            Assert.That(viewModel.Ukprn, Is.EqualTo(ukprn));
            Assert.That(viewModel.PermissionToAddCohorts, Is.EqualTo(SetPermissions.AddRecords.Yes));
            Assert.That(viewModel.PermissionToRecruit, Is.EqualTo(SetPermissions.RecruitApprentices.Yes));
        });
    }

    [Test, MoqAutoData]
    public void Get_SessionModelNotSet_RedirectsToAddEmployerStart(int ukprn)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);
        AddPermissionsAndEmployerController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<AddPermissionsAndEmployerSubmitViewModel>>());

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test]
    [MoqInlineAutoData(null, null)]
    [MoqInlineAutoData("test@test.com", null)]
    [MoqInlineAutoData(null, 134L)]
    public void Get_SessionModelInvalid_RedirectsToAddEmployerStart(string? email, long? accountLegalEntityId, int ukprn)
    {
        var sessionServiceMock = new Mock<ISessionService>();

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);
        AddPermissionsAndEmployerController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<AddPermissionsAndEmployerSubmitViewModel>>());

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.Yes)]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.No)]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.YesWithReview)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.Yes)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.No)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.YesWithReview)]
    public void Get_SessionModelValid_SetsExpectedPermissions(string permissionToAdd, string permissionToRecruit, string email, long accountLegalEntityId, int ukprn)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(
            new AddEmployerSessionModel
            {
                Email = email!,
                AccountLegalEntityId = accountLegalEntityId,
                AccountLegalEntityName = "name",
                PermissionToAddCohorts = permissionToAdd,
                PermissionToRecruit = permissionToRecruit
            });

        AddPermissionsAndEmployerController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<AddPermissionsAndEmployerSubmitViewModel>>());

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        AddPermissionsAndEmployerViewModel? viewModel = viewResult.Model as AddPermissionsAndEmployerViewModel;

        Assert.Multiple(() =>
        {
            Assert.That(viewResult.ViewName, Is.EqualTo(AddPermissionsAndEmployerController.ViewPath));
            Assert.That(viewModel!.PermissionToAddCohorts, Is.EqualTo(permissionToAdd));
            Assert.That(viewModel.PermissionToRecruit, Is.EqualTo(permissionToRecruit));
        });
    }
}
