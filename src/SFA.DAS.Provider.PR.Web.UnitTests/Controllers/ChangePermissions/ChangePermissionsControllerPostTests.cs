using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.ChangePermissions;
public class ChangePermissionsControllerPostTests
{
    private static readonly string CancelLink = Guid.NewGuid().ToString();
    private static readonly string NextPageLink = Guid.NewGuid().ToString();
    private static readonly string CheckDetailsLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Post_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IValidator<ChangePermissionsSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ChangePermissionsController sut,
        AddEmployerSessionModel sessionModel,
        int ukprn,
        ChangePermissionsViewModel viewModel
    )
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(sessionModel);

        validatorMock.Setup(v => v.Validate(It.IsAny<ChangePermissionsSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.CheckEmployerDetails, CheckDetailsLink);

        var result = sut.Index(ukprn, viewModel);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.CheckEmployerDetails);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(s => s.PermissionToAddCohorts == sessionModel.PermissionToAddCohorts && s.PermissionToRecruit == sessionModel.PermissionToRecruit)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_SessionModelNotSet_RedirectsToAddEmployerStart(int ukprn,
        [Frozen] Mock<IValidator<ChangePermissionsSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ChangePermissionsController sut,
        ChangePermissionsViewModel viewModel)
    {
        validatorMock.Setup(v => v.Validate(It.IsAny<ChangePermissionsSubmitModel>())).Returns(new ValidationResult());
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn, viewModel);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public void Post_Invalid_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IValidator<ChangePermissionsSubmitModel>> validatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ChangePermissionsController sut,
        AddEmployerSessionModel sessionModel,
        int ukprn,
        ChangePermissionsSubmitModel submitViewModel
    )
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(sessionModel);

        validatorMock.Setup(m => m.Validate(It.IsAny<ChangePermissionsSubmitModel>())).Returns(new ValidationResult(new List<ValidationFailure>()
        {
            new("TestField","Test Message") { ErrorCode = "1001"}
        }));

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn, submitViewModel);

        ViewResult? viewResult = result.As<ViewResult>();
        ChangePermissionsViewModel? viewModel = viewResult.Model as ChangePermissionsViewModel;

        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(viewResult.ViewName, Is.EqualTo(ChangePermissionsController.ViewPath));
            Assert.That(viewModel!.CancelLink, Is.EqualTo(CancelLink));
            Assert.That(viewModel.Email, Is.EqualTo(sessionModel.Email));
            Assert.That(viewModel.Ukprn, Is.EqualTo(ukprn));
        });
    }
}