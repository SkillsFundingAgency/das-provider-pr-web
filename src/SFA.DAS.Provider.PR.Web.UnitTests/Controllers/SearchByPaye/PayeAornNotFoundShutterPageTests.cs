using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers.SearchByPaye;
public class PayeAornNotFoundShutterPageTests
{
    private static readonly string AddEmployerSearchByPayeLink = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void PayeAornNotCorrectShutterPage_BuildsExpectedViewModel(
        [Greedy] SearchByPayeController sut,
        int ukprn)
    {
        sut.AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerSearchByPaye, AddEmployerSearchByPayeLink);

        var result = sut.PayeAornShutterPage(ukprn);

        ViewResult? viewResult = result.As<ViewResult>();
        PayeAornNotCorrectShutterPageViewModel? viewModel = viewResult.Model as PayeAornNotCorrectShutterPageViewModel;
        viewModel!.BackLink.Should().Be(AddEmployerSearchByPayeLink);
        viewModel!.CheckEmployerLink.Should().Be(AddEmployerSearchByPayeLink);
    }
}
