using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SFA.DAS.Provider.PR.Application.Constants;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;

public sealed class RequestPermissionsConfirmationControllerTests
{
    [Test, AutoData]
    public void RequestPermissionsConfirmation_Index_ReturnsPopulatedModel(long ukprn, string accountLegalEntityId, string employerUrl)
    {
        var sut = new RequestPermissionsConfirmationController();

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        tempData.TryAdd(TempDataKeys.AccountLegalEntityName, "AccountLegalEntityName");
        sut.TempData = tempData;

        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.Employers, employerUrl);

        var actual = sut.Index(ukprn, accountLegalEntityId, CancellationToken.None);

        var viewModel = actual.As<ViewResult>().Model.As<RequestPermissionsConfirmationViewModel>();

        using (new AssertionScope())
        {
            viewModel.EmployersLink.Should().Be(employerUrl);
            viewModel.AccountLegalEntityName.Should().Be("AccountLegalEntityName");
            viewModel.Ukprn.Should().Be(ukprn);
        }
    }

    [Test, AutoData]
    public void RequestPermissionsConfirmation_Index_ReturnsEmptyAccountLegalEntityModel(long ukprn, string accountLegalEntityId, string employerUrl)
    {
        var sut = new RequestPermissionsConfirmationController();

        var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        sut.TempData = tempData;

        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.Employers, employerUrl);

        var actual = sut.Index(ukprn, accountLegalEntityId, CancellationToken.None);

        var viewModel = actual.As<ViewResult>().Model.As<RequestPermissionsConfirmationViewModel>();

        using (new AssertionScope())
        {
            viewModel.EmployersLink.Should().Be(employerUrl);
            viewModel.AccountLegalEntityName.Should().Be(string.Empty);
            viewModel.Ukprn.Should().Be(ukprn);
        }
    }
}
