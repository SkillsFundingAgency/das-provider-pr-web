using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestEase;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByEmail;
public class EmailSearchInviteAlreadySentShutterPageTests
{
    private static readonly string EmployerDetailsLink = Guid.NewGuid().ToString();
    private static readonly string AddEmployerStartLink = Guid.NewGuid().ToString();

    private const string Email = "test@test.com";

    [Test, MoqInlineAutoData]
    public async Task ShutterPage_SessionNotSet_RedirectsToAddEmployerStart(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn)
    {

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = null!
        });

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, AddEmployerStartLink);

        var result = await sut.EmailSearchInviteAlreadySent(ukprn, new CancellationToken());

        ViewResult? viewResult = result.As<ViewResult>();
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqInlineAutoData]
    public async Task ShutterPage_SessionDetailsIncomplete_RedirectsToAddEmployerStart(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SearchByEmailController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, AddEmployerStartLink);

        var result = await sut.EmailSearchInviteAlreadySent(ukprn, new CancellationToken());

        ViewResult? viewResult = result.As<ViewResult>();
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqInlineAutoData]
    public async Task ShutterPage_RequestDetailsEmptyNotSet_RedirectsToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        CancellationToken cancellationToken)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email
        });

        Response<GetRequestByUkprnEmailResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => ((GetRequestByUkprnEmailResponse?)null)!);

        outerApiMock.Setup(o => o.GetRequestByUkprnAndEmail(ukprn, Email, cancellationToken)).ReturnsAsync(resultResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, AddEmployerStartLink);

        var result = await sut.EmailSearchInviteAlreadySent(ukprn, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqInlineAutoData]
    public async Task ShutterPage_RequestDetailsOrganisationNameNotSet_RedirectsToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        GetRequestByUkprnEmailResponse getRequestByUkprnEmailResponse,
        int ukprn,
        CancellationToken cancellationToken)
    {
        getRequestByUkprnEmailResponse.EmployerOrganisationName = string.Empty;
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email
        });

        Response<GetRequestByUkprnEmailResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestByUkprnEmailResponse);

        outerApiMock.Setup(o => o.GetRequestByUkprnAndEmail(ukprn, Email, cancellationToken)).ReturnsAsync(resultResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, AddEmployerStartLink);

        var result = await sut.EmailSearchInviteAlreadySent(ukprn, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqInlineAutoData]
    public async Task ShutterPage_RequestDetailsAccountLegalEntityIdNotSet_RedirectsToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        GetRequestByUkprnEmailResponse getRequestByUkprnEmailResponse,
        int ukprn,
        CancellationToken cancellationToken)
    {
        getRequestByUkprnEmailResponse.AccountLegalEntityId = null;
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email
        });

        Response<GetRequestByUkprnEmailResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestByUkprnEmailResponse);

        outerApiMock.Setup(o => o.GetRequestByUkprnAndEmail(ukprn, Email, cancellationToken)).ReturnsAsync(resultResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, AddEmployerStartLink);

        var result = await sut.EmailSearchInviteAlreadySent(ukprn, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public async Task ShutterPage_BuildsExpectedViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IEncodingService> encodingServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn,
        int accountLegalEntityId,
        string employerName,
        GetRequestByUkprnEmailResponse getRequestByUkprnEmailResponse,
        CancellationToken cancellationToken)
    {
        getRequestByUkprnEmailResponse.EmployerOrganisationName = employerName;
        var email = "test@test.com";
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = email,
            AccountLegalEntityId = accountLegalEntityId,
            AccountLegalEntityName = employerName
        });

        Response<GetRequestByUkprnEmailResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestByUkprnEmailResponse);

        outerApiMock.Setup(o => o.GetRequestByUkprnAndEmail(ukprn, Email, cancellationToken)).ReturnsAsync(resultResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.EmployerDetails, EmployerDetailsLink);

        var result = await sut.EmailSearchInviteAlreadySent(ukprn, cancellationToken);

        ViewResult? viewResult = result.As<ViewResult>();

        EmailSearchInviteAlreadySentShutterPageViewModel? viewModel =
            viewResult.Model as EmailSearchInviteAlreadySentShutterPageViewModel;
        viewModel!.EmployerAccountLink.Should().Be(EmployerDetailsLink);
        viewModel.Email.Should().Be(Email);
        viewModel.EmployerName.Should().Be(employerName.ToUpper());
    }
}
