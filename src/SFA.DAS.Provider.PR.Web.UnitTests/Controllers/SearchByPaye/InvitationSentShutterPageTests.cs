using System.Net;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestEase;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByPaye;
public class InvitationSentShutterPageTests
{
    private static readonly string AddEmployerSearchByPayeLink = Guid.NewGuid().ToString();
    private const string Email = "test@test.com";

    [Test, MoqAutoData]
    public async Task InvitationSentShutterPage_BuildsExpectedViewModel(
        int ukprn,
        string aorn,
        string paye,
        string employerOrganisationName,
        GetRequestByUkprnPayeResponse getRequestByUkprnPayeResponse,
        CancellationToken cancellationToken)
    {
        Mock<ISessionService> sessionServiceMock = new Mock<ISessionService>();
        Mock<IOuterApiClient> outerApiClientMock = new Mock<IOuterApiClient>();

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email,
            Paye = paye,
            Aorn = aorn
        });

        getRequestByUkprnPayeResponse.EmployerOrganisationName = employerOrganisationName;

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestByUkprnPayeResponse);

        outerApiClientMock.Setup(o => o.GetRequest(ukprn, paye, cancellationToken)).ReturnsAsync(resultResponse);

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<SearchByPayeSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink);

        var result = await sut.InvitationSentShutterPage(ukprn, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        InviteAlreadySentShutterPageViewModel? viewModel = viewResult.Model as InviteAlreadySentShutterPageViewModel;

        var expectedViewModel = new InviteAlreadySentShutterPageViewModel(employerOrganisationName, paye, aorn, Email,
            AddEmployerSearchByPayeLink, "");

        viewModel.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task InvitationSentShutterPage_RedirectsToStartIfSessionModelNotSet(
        int ukprn,
        string aorn,
        string employerOrganisationName,
        CancellationToken cancellationToken)
    {
        Mock<ISessionService> sessionServiceMock = new Mock<ISessionService>();

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        var getRequestResponse = new GetRequestByUkprnPayeResponse { EmployerOrganisationName = employerOrganisationName };

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestResponse);

        SearchByPayeController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<SearchByPayeSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink);

        var result = await sut.InvitationSentShutterPage(ukprn, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task InvitationSentShutterPage_RedirectsToStartIfSessionModelPayeNotSet(
        int ukprn,
        string aorn,
        string employerOrganisationName,
        CancellationToken cancellationToken)
    {
        Mock<ISessionService> sessionServiceMock = new Mock<ISessionService>();

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email,
            Paye = null,
            Aorn = aorn
        });
        var getRequestResponse = new GetRequestByUkprnPayeResponse { EmployerOrganisationName = employerOrganisationName };

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestResponse);

        SearchByPayeController sut = new(Mock.Of<IOuterApiClient>(), sessionServiceMock.Object, Mock.Of<IValidator<SearchByPayeSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink);

        var result = await sut.InvitationSentShutterPage(ukprn, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task InvitationSentShutterPage_RedirectsToStartIfResponseCodeIsNotOk(
        int ukprn,
        string paye,
        string aorn,
        string employerOrganisationName,
        CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new Mock<IOuterApiClient>();
        Mock<ISessionService> sessionServiceMock = new Mock<ISessionService>();

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email,
            Paye = paye,
            Aorn = aorn
        });
        var getRequestResponse = new GetRequestByUkprnPayeResponse { EmployerOrganisationName = employerOrganisationName };

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.NotFound), () => getRequestResponse);
        outerApiClientMock.Setup(o => o.GetRequest(ukprn, paye, cancellationToken)).ReturnsAsync(resultResponse);

        SearchByPayeController sut = new(outerApiClientMock.Object, sessionServiceMock.Object, Mock.Of<IValidator<SearchByPayeSubmitViewModel>>());

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink);

        var result = await sut.InvitationSentShutterPage(ukprn, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }
}
