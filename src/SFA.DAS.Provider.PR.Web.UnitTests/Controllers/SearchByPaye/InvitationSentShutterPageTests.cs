using System.Net;
using AutoFixture.NUnit3;
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
    private static readonly string EmployerDetailsLink = Guid.NewGuid().ToString();
    private const string Email = "test@test.com";

    [Test, MoqAutoData]
    public async Task InvitationSentShutterPage_BuildsExpectedViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string aorn,
        string paye,
        string employerOrganisationName,
        int accountLegalEntityId,
        GetRequestByUkprnPayeResponse getRequestByUkprnPayeResponse,
        CancellationToken cancellationToken)
    {
        var createAccount = "CreateAccount";


        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email,
            Paye = paye,
            Aorn = aorn,
            AccountLegalEntityId = accountLegalEntityId
        });

        getRequestByUkprnPayeResponse.EmployerOrganisationName = employerOrganisationName;
        getRequestByUkprnPayeResponse.RequestType = createAccount;

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestByUkprnPayeResponse);

        outerApiClientMock.Setup(o => o.GetRequest(ukprn, paye, cancellationToken)).ReturnsAsync(resultResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink).AddUrlForRoute(RouteNames.EmployerDetails, EmployerDetailsLink);

        var result = await sut.InvitationSentShutterPage(ukprn, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        InviteAlreadySentShutterPageViewModel? viewModel = viewResult.Model as InviteAlreadySentShutterPageViewModel;

        var expectedViewModel = new InviteAlreadySentShutterPageViewModel(employerOrganisationName, paye, aorn, Email,
            AddEmployerSearchByPayeLink, EmployerDetailsLink);

        viewModel.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task InvitationSentShutterPage_RedirectsToStartIfSessionModelNotSet(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string aorn,
        string employerOrganisationName,
        CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        var getRequestResponse = new GetRequestByUkprnPayeResponse { EmployerOrganisationName = employerOrganisationName };

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink);

        var result = await sut.InvitationSentShutterPage(ukprn, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task InvitationSentShutterPage_RedirectsToStartIfSessionModelPayeNotSet(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string aorn,
        string employerOrganisationName,
        CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email,
            Paye = null,
            Aorn = aorn
        });
        var getRequestResponse = new GetRequestByUkprnPayeResponse { EmployerOrganisationName = employerOrganisationName };

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink);

        var result = await sut.InvitationSentShutterPage(ukprn, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task InvitationSentShutterPage_RedirectsToStartIfResponseCodeIsNotOk(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string paye,
        string aorn,
        string employerOrganisationName,
        CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email,
            Paye = paye,
            Aorn = aorn
        });
        var getRequestResponse = new GetRequestByUkprnPayeResponse { EmployerOrganisationName = employerOrganisationName };

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.NotFound), () => getRequestResponse);
        outerApiClientMock.Setup(o => o.GetRequest(ukprn, paye, cancellationToken)).ReturnsAsync(resultResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink);

        var result = await sut.InvitationSentShutterPage(ukprn, cancellationToken);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }
}
