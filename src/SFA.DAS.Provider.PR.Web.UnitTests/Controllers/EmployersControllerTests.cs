using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using RestEase;
using SFA.DAS.Provider.PR.Application.Constants;
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
    public async Task IndexWithUkprn_HasRelationships_ReturnsDefaultView(int ukprn, GetProviderRelationshipsResponse response, GetRequestsByRequestIdResponse responseByRequestData, string clearFilterUrl, string addEmployerUrl, ApplicationSettings applicationSettings, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        response.HasAnyRelationships = true;
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        responseByRequestData.RequestType = RequestType.Permission.ToString();
        Response<GetRequestsByRequestIdResponse> responseByRequest = new(null, new(HttpStatusCode.OK), () => responseByRequestData);
        outerApiClientMock.Setup(c => c.GetRequestByRequestId(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync(responseByRequest);

        Mock<IOptions<ApplicationSettings>> applicationSettingsMock = new();
        applicationSettingsMock.Setup(a => a.Value).Returns(applicationSettings);

        EmployersController sut = new(outerApiClientMock.Object, applicationSettingsMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock();
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

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
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.Should().BeOfType<ViewResult>();
        actual.As<ViewResult>().ViewName.Should().Be(EmployersController.NoRelationshipsHomePage);
        actual.As<ViewResult>().Model.Should().BeOfType<NoRelationshipsHomeViewModel>();
        actual.As<ViewResult>().Model.As<NoRelationshipsHomeViewModel>().AddEmployerLink.Should().Be(addEmployerUrl);
    }

    [Test, AutoData]
    public async Task Index_WithTemporaryRequestIdSet_ClearsValue(long ukprn, GetProviderRelationshipsResponse response, string addEmployerUrl, ApplicationSettings applicationSettings, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();
        response.HasAnyRelationships = false;
        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        Mock<IOptions<ApplicationSettings>> applicationSettingsMock = new();
        applicationSettingsMock.Setup(a => a.Value).Returns(applicationSettings);

        EmployersController sut = new(outerApiClientMock.Object, applicationSettingsMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.AddEmployerStart, addEmployerUrl);

        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        sut.TempData.Add(TempDataKeys.PermissionsRequestId, Guid.NewGuid().ToString());

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        Assert.That(sut.TempData.ContainsKey(TempDataKeys.PermissionsRequestId), Is.False);
    }

    [Test, AutoData]
    public async Task Index_NoRelationshipRequest_BuildsEmployerDetailsLink(int ukprn, GetProviderRelationshipsResponse response, GetRequestsByRequestIdResponse responseByRequestData, string employerDetailsLink, ApplicationSettings applicationSettings, ProviderRelationshipModel employer, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();

        employer.RequestId = null;
        response.Employers = new List<ProviderRelationshipModel> { employer };
        response.HasAnyRelationships = true;

        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        responseByRequestData.RequestType = RequestType.Permission.ToString();
        Response<GetRequestsByRequestIdResponse> responseByRequest = new(null, new(HttpStatusCode.OK), () => responseByRequestData);

        outerApiClientMock.Setup(c => c.GetRequestByRequestId(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync(responseByRequest);

        Mock<IOptions<ApplicationSettings>> applicationSettingsMock = new();
        applicationSettingsMock.Setup(a => a.Value).Returns(applicationSettings);

        EmployersController sut = new(outerApiClientMock.Object, applicationSettingsMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.EmployerDetails, employerDetailsLink);
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.As<ViewResult>().Model.As<EmployersViewModel>().Employers.First().EmployerDetailsUrl.Should().Be(employerDetailsLink);
    }

    [Test, AutoData]
    public async Task Index_HasRelationshipRequests_BuildsEmployerDetailsLink(int ukprn, GetProviderRelationshipsResponse response, GetRequestsByRequestIdResponse responseByRequestData, string employerDetailsLink, ApplicationSettings applicationSettings, ProviderRelationshipModel employer, CancellationToken cancellationToken)
    {
        Mock<IOuterApiClient> outerApiClientMock = new();

        employer.RequestId = Guid.NewGuid();
        response.Employers = new List<ProviderRelationshipModel> { employer };
        response.HasAnyRelationships = true;

        outerApiClientMock.Setup(c => c.GetProviderRelationships(ukprn, It.IsAny<Dictionary<string, string>>(), cancellationToken)).ReturnsAsync(response);

        Response<GetRequestsByRequestIdResponse> responseByRequest = new(null, new(HttpStatusCode.OK), () => responseByRequestData);
        outerApiClientMock.Setup(c => c.GetRequestByRequestId(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync(responseByRequest);

        Mock<IOptions<ApplicationSettings>> applicationSettingsMock = new();
        applicationSettingsMock.Setup(a => a.Value).Returns(applicationSettings);

        EmployersController sut = new(outerApiClientMock.Object, applicationSettingsMock.Object);
        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.EmployerDetailsByRequestId, employerDetailsLink);
        sut.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        var actual = await sut.Index(ukprn, new(), cancellationToken);

        actual.As<ViewResult>().Model.As<EmployersViewModel>().Employers.First().EmployerDetailsUrl.Should().Be(employerDetailsLink);
    }
}
