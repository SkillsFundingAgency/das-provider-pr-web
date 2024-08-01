using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.AddEmployer;
public class StartControllerTests
{
    [Test, AutoData]
    public void ReturnsExpectedStartViewModel(int ukprn, string homeLink, string addSearchByEmailLink)
    {
        StartController sut = new StartController();
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.Home, homeLink).AddUrlForRoute(RouteNames.AddEmployerSearchByEmail, addSearchByEmailLink);
        IActionResult result = sut.Index(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        StartViewModel viewModel = (viewResult.Model as StartViewModel)!;

        viewModel.ContinueLink.Should().Be(addSearchByEmailLink);
        viewModel.ViewEmployersAndPermissionsLink.Should().Be(homeLink);
    }
}
