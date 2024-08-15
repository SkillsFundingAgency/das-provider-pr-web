using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;

public class EmployersControllerTests
{
    [Test, AutoData]
    public async Task IndexWithUkprn_HasRelationships_ReturnsDefaultView(int ukprn, GetProviderRelationshipsResponse response, string clearFilterUrl, string addEmployerUrl, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        response.HasAnyRelationships = true;
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        EmployersController sut = new(outerApiClientMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.Employers, clearFilterUrl).AddUrlForRoute(RouteNames.AddEmployerStart, addEmployerUrl);

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        using (new AssertionScope())
        {
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeOfType<EmployersViewModel>();
            actual.As<ViewResult>().Model.As<EmployersViewModel>();
        }
    }

    [Test, AutoData]
    public async Task IndexWithUkrpn_HasNoRelationships_RetrunsNoRelationshipsView(int ukprn, GetProviderRelationshipsResponse response, string addEmployerUrl, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        response.HasAnyRelationships = false;
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        EmployersController sut = new(outerApiClientMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, addEmployerUrl);

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.Should().BeOfType<ViewResult>();
        actual.As<ViewResult>().ViewName.Should().Be(EmployersController.NoRelationshipsHomePage);
        actual.As<ViewResult>().Model.Should().BeOfType<NoRelationshipsHomeViewModel>();
        actual.As<ViewResult>().Model.As<NoRelationshipsHomeViewModel>().AddEmployerLink.Should().Be(addEmployerUrl);
    }
}
