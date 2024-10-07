﻿using System.Net;
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
public class PayeAornMatchedEmailNotLinkedShutterPageGetTests
{
    private static readonly string StartLink = Guid.NewGuid().ToString();
    private static readonly string ContinueLink = Guid.NewGuid().ToString();
    private const string Email = "test@test.com";

    [Test, MoqAutoData]
    public void ShutterPage_BuildsExpectedViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn,
        string aorn,
        string paye,
        string employerOrganisationName,
        GetRequestByUkprnPayeResponse getRequestByUkprnPayeResponse,
        CancellationToken cancellationToken)
    {
        var createAccount = "CreateAccount";


        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel
        {
            Email = Email,
            Paye = paye,
            Aorn = aorn,
            OrganisationName = employerOrganisationName
        });

        getRequestByUkprnPayeResponse.EmployerOrganisationName = employerOrganisationName;
        getRequestByUkprnPayeResponse.RequestType = createAccount;

        Response<GetRequestByUkprnPayeResponse> resultResponse = new(null, new(HttpStatusCode.OK), () => getRequestByUkprnPayeResponse);

        outerApiClientMock.Setup(o => o.GetRequest(ukprn, paye, cancellationToken)).ReturnsAsync(resultResponse);

        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, StartLink).AddUrlForRoute(RouteNames.AddPermissionsAndEmployer, ContinueLink);

        var result = sut.PayeAornMatchedEmailNotLinkedShutterPage(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        PayeAornMatchedEmailNotLinkedViewModel? viewModel = viewResult.Model as PayeAornMatchedEmailNotLinkedViewModel;

        var expectedViewModel = new PayeAornMatchedEmailNotLinkedViewModel
        {
            EmployerName = employerOrganisationName,
            PayeReference = paye,
            Aorn = aorn,
            Email = Email,
            CancelLink = StartLink,
            ContinueLink = ContinueLink
        };

        viewModel.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public void Get_SessionModelNotSet_RedirectsToAddEmployerStart(
      [Frozen] Mock<IOuterApiClient> outerApiClientMock,
      [Frozen] Mock<ISessionService> sessionServiceMock,
      [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
      [Greedy] SearchByPayeController sut,
      int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns((AddEmployerSessionModel)null!);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, StartLink);

        var result = sut.PayeAornMatchedEmailNotLinkedShutterPage(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public void Get_SessionModelInvalid_RedirectsToAddEmployerStart(
        string email,
        long? accountLegalEntityId,
        [Frozen] Mock<IOuterApiClient> outerApiClientMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByPayeController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email!, Paye = null, AccountLegalEntityId = accountLegalEntityId, AccountLegalEntityName = "name" });

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, StartLink);

        var result = sut.PayeAornMatchedEmailNotLinkedShutterPage(ukprn);

        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }
}
