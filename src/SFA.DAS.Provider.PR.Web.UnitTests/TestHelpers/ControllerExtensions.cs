﻿using System.Security.Claims;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using SFA.DAS.Provider.PR.Web.Authorization;

namespace SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

public static class ControllerExtensions
{
    public static Mock<IUrlHelper> AddUrlHelperMock(this Controller controller)
    {
        var urlHelperMock = new Mock<IUrlHelper>();
        controller.Url = urlHelperMock.Object;
        return urlHelperMock;
    }

    public static Mock<IUrlHelper> AddUrlForRoute(this Mock<IUrlHelper> urlHelperMock, string routeName, string url = TestConstants.DefaultUrl)
    {
        urlHelperMock
           .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName!.Equals(routeName))))
           .Returns(url);
        return urlHelperMock;
    }

    public static Controller AddDefaultContext(this Controller controller)
    {
        Fixture fixture = new();

        var ukprnClaim = new Claim(ProviderClaims.Ukprn, TestConstants.DefaultUkprn.ToString());
        var emailClaim = new Claim(ClaimTypes.Email, fixture.Create<string>());
        var nameClaim = new Claim(ClaimTypes.NameIdentifier, fixture.Create<string>());
        var upnClaim = new Claim(ClaimTypes.Upn, fixture.Create<string>());

        var httpContext = new DefaultHttpContext();
        var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[]
        {
            ukprnClaim,
            emailClaim,
            nameClaim,
            upnClaim
        })});
        httpContext.User = claimsPrinciple;

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        return controller;
    }
}
