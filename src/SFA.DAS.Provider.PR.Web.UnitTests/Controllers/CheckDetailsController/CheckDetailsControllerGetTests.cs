using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.CheckDetailsController;
public class CheckDetailsControllerGetTests
{
    private static readonly string CancelLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModel(int ukprn, AddEmployerSessionModel addEmployerSessionModel)
    {
        var email = "test@test.com";
        addEmployerSessionModel.Email = email;
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        PR.Web.Controllers.AddEmployer.CheckDetailsController sut = new(sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        CheckDetailsViewModel? viewModel = viewResult.Model as CheckDetailsViewModel;

        viewModel.Should().BeEquivalentTo(addEmployerSessionModel, options => options.ExcludingMissingMembers());
        viewModel!.CancelLink.Should().Be(CancelLink);
        viewModel.ChangeEmployerNameLink.Should().Be(string.Empty);
        viewModel.ChangePermissionsLink.Should().Be(string.Empty);
        viewModel.PerimissionToRecruitText.Should().Be(SetPermissionsText.NoPermissionText);
        viewModel.PermissionToAddCohortsText.Should().Be(SetPermissionsText.NoPermissionText);
    }

    [Test, MoqAutoData]
    public void Get_RedirectsToStartIfSessionNotSet(int ukprn)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        PR.Web.Controllers.AddEmployer.CheckDetailsController sut = new(sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public void Get_RedirectsToStartIfEmailNotSetInSession(int ukprn)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        PR.Web.Controllers.AddEmployer.CheckDetailsController sut = new(sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test]
    [MoqInlineAutoData(null, null, 2)]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, null, 1)]
    [MoqInlineAutoData(null, SetPermissions.RecruitApprentices.Yes, 1)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.Yes, 0)]
    public void Get_SetsSessionModelIfUpdatingPermissions(string? permissionToAddCohorts, string? permissionsToRecruit, int numberOfSessionSets, int ukprn)
    {
        var email = "test@test.com";
        AddEmployerSessionModel addEmployerSessionModel = new AddEmployerSessionModel { Email = email };
        addEmployerSessionModel.PermissionToAddCohorts = permissionToAddCohorts;
        addEmployerSessionModel.PermissionToRecruit = permissionsToRecruit;

        addEmployerSessionModel.Email = email;
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        PR.Web.Controllers.AddEmployer.CheckDetailsController sut = new(sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        sut.Index(ukprn);

        sessionServiceMock.Verify(x => x.Set(It.IsAny<AddEmployerSessionModel>()), Times.Exactly(numberOfSessionSets));
    }

    [Test]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, SetPermissionsText.CohortsPermissionText)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissionsText.NoPermissionText)]
    [MoqInlineAutoData(null, SetPermissionsText.CohortsPermissionText)]
    public void Get_SetsPermissionsToAddCohortText(string? permissionToAddCohorts, string permissionsToAddCohortsText, int ukprn)
    {
        var email = "test@test.com";
        AddEmployerSessionModel addEmployerSessionModel = new AddEmployerSessionModel
        {
            Email = email,
            PermissionToAddCohorts = permissionToAddCohorts
        };

        addEmployerSessionModel.Email = email;
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        PR.Web.Controllers.AddEmployer.CheckDetailsController sut = new(sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);
        ViewResult? viewResult = result.As<ViewResult>();
        CheckDetailsViewModel? viewModel = viewResult.Model as CheckDetailsViewModel;

        viewModel!.PermissionToAddCohortsText.Should().Be(permissionsToAddCohortsText);
    }

    [Test]
    [MoqInlineAutoData(SetPermissions.RecruitApprentices.Yes, SetPermissionsText.RecruitmentPermissionText)]
    [MoqInlineAutoData(SetPermissions.RecruitApprentices.YesWithReview, SetPermissionsText.RecruitmentWithReviewPermissionText)]
    [MoqInlineAutoData(SetPermissions.RecruitApprentices.No, SetPermissionsText.NoPermissionText)]
    [MoqInlineAutoData(null, SetPermissionsText.RecruitmentPermissionText)]
    public void Get_SetsPermissionsToRecruitText(string? permission, string permissionText, int ukprn)
    {
        var email = "test@test.com";
        AddEmployerSessionModel addEmployerSessionModel = new AddEmployerSessionModel
        {
            Email = email,
            PermissionToRecruit = permission
        };

        addEmployerSessionModel.Email = email;
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        PR.Web.Controllers.AddEmployer.CheckDetailsController sut = new(sessionServiceMock.Object);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);
        ViewResult? viewResult = result.As<ViewResult>();
        CheckDetailsViewModel? viewModel = viewResult.Model as CheckDetailsViewModel;

        viewModel!.PerimissionToRecruitText.Should().Be(permissionText);
    }

}
