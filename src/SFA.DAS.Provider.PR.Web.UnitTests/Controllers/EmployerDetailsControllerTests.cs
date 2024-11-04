using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Controllers;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Controllers;
public class EmployerDetailsControllerTests
{
    [Test, AutoData]
    public async Task IndexWithUkprnAndAccountlegalentityid_ReturnsDefaultView(int ukprn,
        string hashedAccountLegalEntityId, long accountLegalEntityId, GetProviderRelationshipResponse response)
    {
        Mock<IEncodingService> encodingService = new();
        encodingService.Setup(x => x.Decode(hashedAccountLegalEntityId, EncodingType.PublicAccountLegalEntityId))
            .Returns(accountLegalEntityId);

        Mock<IOuterApiClient> outerApiClientMock = new();
        outerApiClientMock.Setup(x => x.GetProviderRelationship(ukprn, accountLegalEntityId, CancellationToken.None))
            .ReturnsAsync(response);

        EmployerDetailsController sut = new(outerApiClientMock.Object, encodingService.Object);

        sut.AddDefaultContext().AddUrlHelperMock();

        var actual = await sut.Index(ukprn, hashedAccountLegalEntityId, CancellationToken.None);

        using (new AssertionScope())
        {
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeOfType<EmployerDetailsViewModel>();
            actual.As<ViewResult>().Model.As<EmployerDetailsViewModel>();
        }
    }

    [Test, AutoData]
    public async Task IndexWithUkprnAndAccountLegalEntityId_BuildsEmployerLinkCorrectly(int ukprn,
        string hashedAccountLegalEntityId, long accountLegalEntityId, string employerUrl, GetProviderRelationshipResponse response)
    {
        Mock<IEncodingService> encodingService = new();
        encodingService.Setup(x => x.Decode(hashedAccountLegalEntityId, EncodingType.PublicAccountLegalEntityId))
            .Returns(accountLegalEntityId);

        Mock<IOuterApiClient> outerApiClientMock = new();
        outerApiClientMock.Setup(x => x.GetProviderRelationship(ukprn, accountLegalEntityId, CancellationToken.None))
            .ReturnsAsync(response);

        EmployerDetailsController sut = new(outerApiClientMock.Object, encodingService.Object);

        sut.AddDefaultContext().AddUrlHelperMock().AddUrlForRoute(RouteNames.Employers, employerUrl);

        var actual = await sut.Index(ukprn, hashedAccountLegalEntityId, CancellationToken.None);

        actual.As<ViewResult>().Model.As<EmployerDetailsViewModel>().EmployersLink.Should().Be(employerUrl);

    }
}
