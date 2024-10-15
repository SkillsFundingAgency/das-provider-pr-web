using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;

public class EmployersControllerTests
{
    [Test, AutoData]
    public async Task IndexWithUkprn_HasRelationships_ReturnsDefaultView(int ukprn, GetProviderRelationshipsResponse response, GetRequestsByRequestIdResponse responseByRequest, string clearFilterUrl, string addEmployerUrl, ApplicationSettings applicationSettings, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        response.HasAnyRelationships = true;
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        responseByRequest.RequestType = "Permission";
        outerApiClientMock.Setup(c => c.GetRequestByRequestId(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync(responseByRequest);

        Mock<IOptions<ApplicationSettings>> applicationSettingsMock = new();
        applicationSettingsMock.Setup(a => a.Value).Returns(applicationSettings);

        EmployersController sut = new(outerApiClientMock.Object, applicationSettingsMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock();

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
    public async Task IndexWithUkrpn_HasNoRelationships_RetrunsNoRelationshipsView(int ukprn, GetProviderRelationshipsResponse response, string addEmployerUrl, ApplicationSettings applicationSettings, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        response.HasAnyRelationships = false;
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        Mock<IOptions<ApplicationSettings>> applicationSettingsMock = new();
        applicationSettingsMock.Setup(a => a.Value).Returns(applicationSettings);

        EmployersController sut = new(outerApiClientMock.Object, applicationSettingsMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, addEmployerUrl);

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.Should().BeOfType<ViewResult>();
        actual.As<ViewResult>().ViewName.Should().Be(EmployersController.NoRelationshipsHomePage);
        actual.As<ViewResult>().Model.Should().BeOfType<NoRelationshipsHomeViewModel>();
        actual.As<ViewResult>().Model.As<NoRelationshipsHomeViewModel>().AddEmployerLink.Should().Be(addEmployerUrl);
    }

    [Test, AutoData]
    public async Task IndexWithUkprn_HasExistingRelationships_BuildsEmployerDetailsLink(int ukprn, GetProviderRelationshipsResponse response, GetRequestsByRequestIdResponse responseByRequest, string employerDetailsLink, ApplicationSettings applicationSettings, ProviderRelationshipModel employer, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();

        employer.RequestId = null;
        response.Employers = new List<ProviderRelationshipModel> { employer };
        response.HasAnyRelationships = true;

        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        responseByRequest.RequestType = "Permission";
        outerApiClientMock.Setup(c => c.GetRequestByRequestId(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync(responseByRequest);

        Mock<IOptions<ApplicationSettings>> applicationSettingsMock = new();
        applicationSettingsMock.Setup(a => a.Value).Returns(applicationSettings);

        EmployersController sut = new(outerApiClientMock.Object, applicationSettingsMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.EmployerDetails, employerDetailsLink);

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.As<ViewResult>().Model.As<EmployersViewModel>().Employers.First().EmployerDetailsUrl.Should().Be(employerDetailsLink);
    }

    [Test]
    [InlineAutoData("AddAccount")]
    [InlineAutoData("CreateAccount")]
    public async Task IndexWithUkprn_HasRelationshipRequests_BuildsEmployerDetailsLink(string requestType, int ukprn, GetProviderRelationshipsResponse response, GetRequestsByRequestIdResponse responseByRequest, string employerDetailsLink, ApplicationSettings applicationSettings, ProviderRelationshipModel employer, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();

        employer.RequestId = new Guid();
        response.Employers = new List<ProviderRelationshipModel> { employer };
        response.HasAnyRelationships = true;

        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        responseByRequest.RequestType = requestType;
        outerApiClientMock.Setup(c => c.GetRequestByRequestId(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync(responseByRequest);

        Mock<IOptions<ApplicationSettings>> applicationSettingsMock = new();
        applicationSettingsMock.Setup(a => a.Value).Returns(applicationSettings);

        EmployersController sut = new(outerApiClientMock.Object, applicationSettingsMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.EmployerDetails, employerDetailsLink);

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.As<ViewResult>().Model.As<EmployersViewModel>().Employers.First().EmployerDetailsUrl.Should().Be(employerDetailsLink);
        outerApiClientMock.Verify(x => x.GetRequestByRequestId((Guid)employer.RequestId, cancellationToken), Times.AtLeast(1));
    }
}
