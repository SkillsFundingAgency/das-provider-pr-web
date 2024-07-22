using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;

public class ErrorControllerTests
{
    [TestCase(403, "AccessDenied")]
    [TestCase(404, "PageNotFound")]
    [TestCase(500, "ErrorInService")]
    public void HttpStatusCodeHandler_ReturnsViewForStatusCode(int statusCode, string viewName)
    {
        Fixture fixture = new Fixture();
        var sut = GetSut(fixture.Create<ApplicationSettings>(), fixture.Create<ProviderSharedUIConfiguration>(), new DefaultHttpContext());

        var actual = sut.HttpStatusCodeHandler(statusCode);

        actual.Should().BeOfType<ViewResult>();
        actual.As<ViewResult>().ViewName.Should().Be(viewName);
    }

    [Test, AutoData]
    public void ErrorInService_ReturnsView(string path, ApplicationSettings applicationSettings, ProviderSharedUIConfiguration sharedUiSettings)
    {
        Mock<IExceptionHandlerPathFeature> exceptionHandlerFeatureMock = new();
        exceptionHandlerFeatureMock.Setup(e => e.Path).Returns(path);
        Mock<IFeatureCollection> featureCollectionMock = new();
        featureCollectionMock.Setup(f => f.Get<IExceptionHandlerPathFeature>()).Returns(exceptionHandlerFeatureMock.Object);
        var httpContext = new DefaultHttpContext(featureCollectionMock.Object);
        var sut = GetSut(applicationSettings, sharedUiSettings, httpContext);

        var result = sut.ErrorInService();

        using (new AssertionScope())
        {
            result.Should().BeOfType<ViewResult>();
            var actual = result.As<ViewResult>().Model.As<ErrorViewModel>();
            actual.Should().NotBeNull();
            actual.HomePageUrl.Should().Be(sharedUiSettings.DashboardUrl);
            actual.HelpPageUrl.Should().Be(applicationSettings.DfESignInServiceHelpUrl);
        }
    }

    private static ErrorController GetSut(ApplicationSettings applicationSettings, ProviderSharedUIConfiguration sharedUiSettings, HttpContext httpContext)
    {
        Mock<IOptions<ApplicationSettings>> applicationSettingsOptionsMock = new();
        applicationSettingsOptionsMock.Setup(o => o.Value).Returns(applicationSettings);

        Mock<IOptions<ProviderSharedUIConfiguration>> sharedUiSettingsOptionsMock = new();
        sharedUiSettingsOptionsMock.Setup(o => o.Value).Returns(sharedUiSettings);

        return new(Mock.Of<ILogger<ErrorController>>(), applicationSettingsOptionsMock.Object, sharedUiSettingsOptionsMock.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext }
        };
    }
}
