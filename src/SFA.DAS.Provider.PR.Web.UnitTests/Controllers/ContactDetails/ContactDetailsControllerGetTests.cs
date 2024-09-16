using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.ContactDetails;
public class ContactDetailsControllerGetTests
{
    private static readonly string BackLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = Guid.NewGuid().ToString();
    private static readonly string DetailsLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
        int ukprn,
        string aorn,
        string paye)
    {
        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, BackLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        ContactDetailsViewModel? viewModel = viewResult.Model as ContactDetailsViewModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.Paye.Should().Be(paye);
        viewModel.Aorn.Should().Be(aorn);
        viewModel.FirstName.Should().BeNull();
        viewModel.LastName.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void Get_BuildsExpectedViewModel_BackLinkToCheckDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
        int ukprn,
        string aorn,
        string paye)
    {
        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, Paye = paye, Aorn = aorn, IsCheckDetailsVisited = true });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.CheckEmployerDetails, DetailsLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        ContactDetailsViewModel? viewModel = viewResult.Model as ContactDetailsViewModel;

        viewModel!.BackLink.Should().Be(DetailsLink);
        viewModel.CancelLink.Should().Be(CancelLink);
    }

    [Test, MoqAutoData]
    public void Get_FirstNameLastNameInSession_BuildsExpectedViewModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
        int ukprn,
        string firstName,
        string lastName)
    {
        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email, FirstName = firstName, LastName = lastName });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, BackLink);

        var result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        ContactDetailsViewModel? viewModel = viewResult.Model as ContactDetailsViewModel;
        viewModel!.Ukprn.Should().Be(ukprn);
        viewModel.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.FirstName.Should().Be(firstName);
        viewModel.LastName.Should().Be(lastName);
    }

    [Test, MoqAutoData]
    public void Get_RedirectsToStartIfSessionNotSet(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
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
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ContactDetailsController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink);

        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.AddEmployerStart);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
    }
}