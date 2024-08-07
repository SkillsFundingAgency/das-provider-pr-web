﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;

public class HomeControllerTests
{
    [Test]
    public void Index_RedirectsToIndexWithUkprn()
    {
        HomeController sut = new(Mock.Of<IOuterApiClient>());
        sut.AddDefaultContext();

        var actual = sut.Index();

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.Home);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
        actual.As<RedirectToRouteResult>().RouteValues!["ukprn"].Should().Be(TestConstants.DefaultUkprn.ToString());
    }

    [Test, AutoData]
    public async Task IndexWithUkprn_HasRelationships_ReturnsHomeView(int ukprn, GetProviderRelationshipsResponse response, string clearFilterUrl, string addEmployerUrl, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        response.HasAnyRelationships = true;
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        HomeController sut = new(outerApiClientMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.Home, clearFilterUrl).AddUrlForRoute(RouteNames.AddEmployerStart, addEmployerUrl);

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.Should().BeOfType<ViewResult>();
        actual.As<ViewResult>().ViewName.Should().BeNull();
        actual.As<ViewResult>().Model.Should().BeOfType<HomeViewModel>();
    }

    [Test, AutoData]
    public async Task IndexWithUkrpn_HasNoRelationships_RetrunsNoRelationshipsView(int ukprn, GetProviderRelationshipsResponse response, string addEmployerUrl, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        response.HasAnyRelationships = false;
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        HomeController sut = new(outerApiClientMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, addEmployerUrl);

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.Should().BeOfType<ViewResult>();
        actual.As<ViewResult>().ViewName.Should().Be(HomeController.NoRelationshipsHomePage);
        actual.As<ViewResult>().Model.Should().BeOfType<NoRelationshipsHomeViewModel>();
        actual.As<ViewResult>().Model.As<NoRelationshipsHomeViewModel>().AddEmployerLink.Should().Be(addEmployerUrl);
    }
}
