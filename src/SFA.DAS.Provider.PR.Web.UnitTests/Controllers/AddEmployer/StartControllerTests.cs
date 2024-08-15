using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.AddEmployer;
public class StartControllerTests
{
    [Test, AutoData]
    public void ReturnsExpectedStartViewModel(int ukprn, string homeLink, string addSearchByEmailLink)
    {
        var sessionServiceMock = new Mock<ISessionService>();
        StartController sut = new StartController(sessionServiceMock.Object);
        sut
            .AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.Employers, homeLink)
            .AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, addSearchByEmailLink);

        IActionResult result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        StartViewModel viewModel = (viewResult.Model as StartViewModel)!;

        viewModel.ContinueLink.Should().Be(addSearchByEmailLink);
        viewModel.ViewEmployersAndPermissionsLink.Should().Be(homeLink);
        sessionServiceMock.Verify(s => s.Delete<AddEmployerSessionModel>(), Times.Once);
    }
}
