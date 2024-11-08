using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;
public class RequestPermissionsControllerTests
{
    [Test, AutoData]
    public async Task PermissionsRequested_ModelIsBuiltCorrectly(int ukprn, string employerUrl)
    {
        var sut = new RequestPermissionsController();

        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.Employers, employerUrl);

        var actual = await sut.PermissionsRequested(ukprn, CancellationToken.None);

        var viewModel = actual.As<ViewResult>().Model.As<RequestPermissionsConfirmationViewModel>();

        using (new AssertionScope())
        {
            viewModel.EmployersLink.Should().Be(employerUrl);
            viewModel.AccountLegalEntityName.Should().Be("TEMP NAME"); //To be changed when controller is fully implemented
            viewModel.Ukprn.Should().Be(ukprn);
        }
    }
}
