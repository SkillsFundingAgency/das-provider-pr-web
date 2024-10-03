using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByEmail;
public class AddPermissionsAndEmployerControllerPostTests
{
    private static readonly string CancelLink = Guid.NewGuid().ToString();
    private static readonly string NextPageLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Post_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<AddPermissionsAndEmployerSubmitModel>> validatorMock,
        [Greedy] AddPermissionsAndEmployerController sut,
        AddEmployerSessionModel sessionModel,
        int ukprn,
        AddPermissionsAndEmployerViewModel addPermissionsAndEmployerViewModel
    )
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(sessionModel);

        validatorMock.Setup(v => v.Validate(It.IsAny<AddPermissionsAndEmployerSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, NextPageLink);

        var result = sut.Index(ukprn, addPermissionsAndEmployerViewModel);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerConfirmation);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);

        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
        sessionServiceMock.Verify(s => s.Set(It.Is<AddEmployerSessionModel>(s => s.PermissionToAddCohorts == sessionModel.PermissionToAddCohorts
                                                                               && s.PermissionToRecruit == sessionModel.PermissionToRecruit)), Times.Once);
    }

    [Test, MoqAutoData]
    public void Post_SessionModelNotSet_RedirectsToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<AddPermissionsAndEmployerSubmitModel>> validatorMock,
        [Greedy] AddPermissionsAndEmployerController sut,
        int ukprn,
        AddPermissionsAndEmployerViewModel addPermissionsAndEmployerViewModel)
    {
        validatorMock.Setup(v => v.Validate(It.IsAny<AddPermissionsAndEmployerSubmitModel>())).Returns(new ValidationResult());

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn, addPermissionsAndEmployerViewModel);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public void Post_Invalid_ReturnsExpectedViewModelAndPath(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<AddPermissionsAndEmployerSubmitModel>> validatorMock,
        [Greedy] AddPermissionsAndEmployerController sut,
        AddEmployerSessionModel sessionModel,
        int ukprn,
        AddPermissionsAndEmployerViewModel addPermissionsAndEmployerViewModel
    )
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(sessionModel);

        validatorMock.Setup(m => m.Validate(It.IsAny<AddPermissionsAndEmployerSubmitModel>())).Returns(new ValidationResult(new List<ValidationFailure>()
        {
            new("TestField","Test Message") { ErrorCode = "1001"}
        }));

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink);

        var result = sut.Index(ukprn, addPermissionsAndEmployerViewModel);

        ViewResult? viewResult = result.As<ViewResult>();
        AddPermissionsAndEmployerViewModel? viewModel = viewResult.Model as AddPermissionsAndEmployerViewModel;

        sessionServiceMock.Verify(s => s.Get<AddEmployerSessionModel>(), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(viewResult.ViewName, Is.EqualTo(AddPermissionsAndEmployerController.ViewPath));
            Assert.That(viewModel!.CancelLink, Is.EqualTo(CancelLink));
            Assert.That(viewModel.Email, Is.EqualTo(sessionModel.Email));
            Assert.That(viewModel.LegalName, Is.EqualTo(sessionModel.AccountLegalEntityName));
            Assert.That(viewModel.Ukprn, Is.EqualTo(ukprn));
        });
    }
}
