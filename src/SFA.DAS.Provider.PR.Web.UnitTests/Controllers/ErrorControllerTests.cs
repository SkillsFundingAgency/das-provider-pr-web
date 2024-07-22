using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;

public class ErrorControllerTests
{
    [TestCase(403, "AccessDenied")]
    [TestCase(404, "PageNotFound")]
    [TestCase(500, "ErrorInService")]
    public void HttpStatusCodeHandler_ReturnsViewForStatusCode(int statusCode, string viewName)
    {
        Fixture fixture = new Fixture();
        Mock<IOptions<ApplicationSettings>> optionsMock = new();
        optionsMock.Setup(o => o.Value).Returns(fixture.Create<ApplicationSettings>());

        ErrorController sut = new(Mock.Of<ILogger<ErrorController>>(), optionsMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
            Url = Mock.Of<IUrlHelper>()
        };

        var actual = sut.HttpStatusCodeHandler(statusCode);

        actual.Should().BeOfType<ViewResult>();
        actual.As<ViewResult>().ViewName.Should().Be(viewName);
    }

    [Test, AutoData]
    public void ErrorInService_ReturnsView(string path, string url, ApplicationSettings applicationSettings)
    {
        Mock<IOptions<ApplicationSettings>> optionsMock = new();
        optionsMock.Setup(o => o.Value).Returns(applicationSettings);
        Mock<IExceptionHandlerPathFeature> exceptionHandlerFeatureMock = new();
        exceptionHandlerFeatureMock.Setup(e => e.Path).Returns(path);
        Mock<IFeatureCollection> featureCollectionMock = new();
        featureCollectionMock.Setup(f => f.Get<IExceptionHandlerPathFeature>()).Returns(exceptionHandlerFeatureMock.Object);
        Mock<IUrlHelper> urlHelperMock = new();
        urlHelperMock.Setup(u => u.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName!.Equals(RouteNames.Home)))).Returns(url);
        ErrorController sut = new(Mock.Of<ILogger<ErrorController>>(), optionsMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext(featureCollectionMock.Object) },
            Url = urlHelperMock.Object
        };

        var result = sut.ErrorInService();

        using (new AssertionScope())
        {
            result.Should().BeOfType<ViewResult>();
            var actual = result.As<ViewResult>().Model.As<ErrorViewModel>();
            actual.Should().NotBeNull();
            actual.HomePageUrl.Should().Be(url);
            actual.HelpPageUrl.Should().Be(applicationSettings.DfESignInServiceHelpUrl);
        }
    }
}
