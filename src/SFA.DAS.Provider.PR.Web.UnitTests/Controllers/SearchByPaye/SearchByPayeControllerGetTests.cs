using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
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

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByPaye;
public class SearchByPayeControllerGetTests
{
    private static readonly string BackLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = Guid.NewGuid().ToString();
    private static readonly string CheckDetailsLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn)
    {
        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        SearchByPayeModel? viewModel = viewResult.Model as SearchByPayeModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.Paye.Should().BeNull();
        viewModel.Aorn.Should().BeNull();
        viewModel.Email.Should().Be(email);
    }

    [Test, MoqAutoData]
    public void Get_AddsExpectedPayeAndAornToViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn)
    {
        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var result = sut.Index(ukprn);

        SearchByPayeModel? viewModel = ((ViewResult)result).Model as SearchByPayeModel;

        viewModel.Should().NotBeNull();
        viewModel!.Paye.Should().Be(paye);
        viewModel.Aorn.Should().Be(aorn);
    }

    [Test, MoqAutoData]
    public void Get_RedirectsToStartIfSessionNotSet(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }


    [Test, MoqAutoData]
    public void Get_RedirectsToStartIfEmailNotSetInSession(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }

    [Test, MoqAutoData]
    public void Get_HasVisitedCheckDetails_RedirectToCheckDetails(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByPayeSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn)
    {
        var email = "test@test.com";
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, IsCheckDetailsVisited = true });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, BackLink).AddUrlForRoute(RouteNames.CheckEmployerDetails, CheckDetailsLink);

        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.CheckEmployerDetails);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }
}
