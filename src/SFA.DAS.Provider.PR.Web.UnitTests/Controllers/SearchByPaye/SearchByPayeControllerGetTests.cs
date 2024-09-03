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
    private static readonly string CancelLink = Guid.Empty.ToString();

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModel(int ukprn)
    {
        var email = "test@test.com";
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email });

        SearchByPayeController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<SearchByPayeSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);


        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        SearchByPayeViewModel? viewModel = viewResult.Model as SearchByPayeViewModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.Paye.Should().BeNull();
        viewModel.Aorn.Should().BeNull();
        viewModel.Email.Should().Be(email);
    }

    [Test, MoqAutoData]
    public void Get_AddsExpectedPayeAndAornToViewModel(int ukprn, string paye, string aorn)
    {
        var email = "test@test.com";
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        SearchByPayeController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<SearchByPayeSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var result = sut.Index(ukprn);

        SearchByPayeViewModel? viewModel = ((ViewResult)result).Model as SearchByPayeViewModel;

        viewModel.Should().NotBeNull();
        viewModel!.Paye.Should().Be(paye);
        viewModel.Aorn.Should().Be(aorn);
    }

    [Test, MoqAutoData]
    public void Get_RedirectsToStartIfSessionNotSet(int ukprn)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        SearchByPayeController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<SearchByPayeSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);


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

        SearchByPayeController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<SearchByPayeSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);


        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }
}
