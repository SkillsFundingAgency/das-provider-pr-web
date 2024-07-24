using AutoFixture.NUnit3;
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
    public async Task IndexWithUkprn_ReturnsView(int ukprn, GetProviderRelationshipsResponse response, string url, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        HomeController sut = new(outerApiClientMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.Home, url);

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.Should().BeOfType<ViewResult>();
        actual.As<ViewResult>().Model.Should().BeOfType<HomeViewModel>();
    }
}
