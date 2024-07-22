using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.AddEmployer;
public class StartControllerTests
{
    [Test]
    public void ReturnsExpectedStartViewModel()
    {
        StartController sut = new StartController();
        IActionResult result = sut.Index();

        ViewResult? viewResult = result.As<ViewResult>();
        StartViewModel viewModel = (viewResult.Model as StartViewModel)!;

        viewModel.ContinueLink.Should().Be("#");
        viewModel.ViewEmployersAndPermissionsLink.Should().Be("#");
    }
}
