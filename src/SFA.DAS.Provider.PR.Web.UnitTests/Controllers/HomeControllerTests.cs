using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;

public class HomeControllerTests
{
    [Test]
    public void Index_RedirectsToIndex()
    {
        HomeController sut = new();
        sut.AddDefaultContext();

        var actual = sut.Index();

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.Employers);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
        actual.As<RedirectToRouteResult>().RouteValues!["ukprn"].Should().Be(TestConstants.DefaultUkprn.ToString());
    }

    [Test, AutoData]
    public void Index_RedirectsToIndexWithUkprn(int ukprn)
    {
        HomeController sut = new();
        sut.AddDefaultContext();

        var actual = sut.Index(ukprn);

        actual.Should().BeOfType<RedirectToRouteResult>();
        actual.As<RedirectToRouteResult>().RouteName.Should().Be(RouteNames.Employers);
        actual.As<RedirectToRouteResult>().RouteValues.Should().ContainKey("ukprn");
        actual.As<RedirectToRouteResult>().RouteValues!["ukprn"].Should().Be(ukprn);
    }
}
