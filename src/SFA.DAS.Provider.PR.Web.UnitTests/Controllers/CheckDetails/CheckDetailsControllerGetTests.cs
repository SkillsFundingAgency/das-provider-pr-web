using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.CheckDetails;
public class CheckDetailsControllerGetTests
{
    private static readonly string CancelLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn,
        AddEmployerSessionModel addEmployerSessionModel)
    {
        var email = "test@test.com";
        addEmployerSessionModel.Email = email;
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        CheckDetailsViewModel? viewModel = viewResult.Model as CheckDetailsViewModel;

        viewModel.Should().BeEquivalentTo(addEmployerSessionModel, options => options.ExcludingMissingMembers()
                                                                                        .Excluding(o => o.OrganisationName));
        viewModel!.OrganisationName.Should().Be(addEmployerSessionModel.OrganisationName!.ToUpper());
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.ChangeEmployerNameLink.Should().Be(string.Empty);
        viewModel.ChangePermissionsLink.Should().Be(string.Empty);
        viewModel.PermissionToRecruitText.Should().Be(SetPermissionsText.NoPermissionText);
        viewModel.PermissionToAddCohortsText.Should().Be(SetPermissionsText.NoPermissionText);
    }

    [Test, MoqAutoData]
    public void Get_RedirectsToStartIfSessionNotSet(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public void Get_RedirectsToStartIfEmailNotSetInSession(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

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
    public void Get_SetsSessionModelIfUpdatingPermissions(
        string? permissionToAddCohorts,
        string? permissionsToRecruit,
        int numberOfSessionSets,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn)
    {
        var email = "test@test.com";
        AddEmployerSessionModel addEmployerSessionModel = new AddEmployerSessionModel { Email = email };
        addEmployerSessionModel.PermissionToAddCohorts = permissionToAddCohorts;
        addEmployerSessionModel.PermissionToRecruit = permissionsToRecruit;

        addEmployerSessionModel.Email = email;
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        sut.Index(ukprn);

        sessionServiceMock.Verify(x => x.Set(It.IsAny<AddEmployerSessionModel>()), Times.Exactly(numberOfSessionSets));
    }

    [Test]
    [MoqInlineAutoData(SetPermissions.AddRecords.Yes, SetPermissionsText.CohortsPermissionText)]
    [MoqInlineAutoData(SetPermissions.AddRecords.No, SetPermissionsText.NoPermissionText)]
    [MoqInlineAutoData(null, SetPermissionsText.CohortsPermissionText)]
    public void Get_SetsPermissionsToAddCohortText(
        string? permissionToAddCohorts,
        string permissionsToAddCohortsText,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn)
    {
        var email = "test@test.com";
        AddEmployerSessionModel addEmployerSessionModel = new AddEmployerSessionModel
        {
            Email = email,
            PermissionToAddCohorts = permissionToAddCohorts
        };

        addEmployerSessionModel.Email = email;
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

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
    public void Get_SetsPermissionsToRecruitText(
        string? permission,
        string permissionText,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] CheckDetailsController sut,
        int ukprn)
    {
        var email = "test@test.com";
        AddEmployerSessionModel addEmployerSessionModel = new AddEmployerSessionModel
        {
            Email = email,
            PermissionToRecruit = permission
        };

        addEmployerSessionModel.Email = email;
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(addEmployerSessionModel);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn);
        ViewResult? viewResult = result.As<ViewResult>();
        CheckDetailsViewModel? viewModel = viewResult.Model as CheckDetailsViewModel;

        viewModel!.PermissionToRecruitText.Should().Be(permissionText);
    }
}
