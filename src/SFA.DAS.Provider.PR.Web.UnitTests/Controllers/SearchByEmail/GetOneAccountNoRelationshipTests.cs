﻿using AutoFixture.NUnit3;
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

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByEmail;
public class GetOneAccountNoRelationshipTests
{
    private static readonly string BackLink = Guid.NewGuid().ToString();
    private static readonly string CancelLink = BackLink;
    private static readonly string ContinueLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void OneAccountNoRelationshipFound_BuildsExpectedViewModel(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn)
    {
        var email = "test@test.com";

        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = email });

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.AddEmployerStart, CancelLink)
            .AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, BackLink)
            .AddUrlForRoute(RouteNames.AddPermissionsAndEmployer, ContinueLink);

        var result = sut.OneAccountNoRelationshipFound(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        OneAccountNoRelationshipViewModel? viewModel = viewResult.Model as OneAccountNoRelationshipViewModel;
        viewModel!.BackLink.Should().Be(BackLink);
        viewModel.CancelLink.Should().Be(CancelLink);
        viewModel.ContinueLink.Should().Be(ContinueLink);
        viewModel.Email.Should().Be(email);
        viewModel.Ukprn.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public void SessionModelEmpty_RedirectToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn)
    {
        var result = sut.OneAccountNoRelationshipFound(ukprn);
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }

    [Test, MoqAutoData]
    public void SessionModelEmptyEmailAddress_RedirectToAddEmployerStart(
        [Frozen] Mock<IOuterApiClient> outerApiMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Frozen] Mock<IValidator<SearchByEmailSubmitModel>> validatorMock,
        [Greedy] SearchByEmailController sut,
        int ukprn)
    {
        sessionServiceMock.Setup(s => s.Get<AddEmployerSessionModel>()).Returns(new AddEmployerSessionModel { Email = string.Empty });

        var result = sut.OneAccountNoRelationshipFound(ukprn);
        RedirectToRouteResult? redirectToRouteResult = result.As<RedirectToRouteResult>();
        redirectToRouteResult.RouteName.Should().Be(RouteNames.AddEmployerStart);
        redirectToRouteResult.RouteValues!.First().Value.Should().Be(ukprn);
    }
}
