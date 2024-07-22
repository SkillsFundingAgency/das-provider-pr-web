using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Extensions;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Extensions;
public class ClaimsPrincipalExtensionsTests
{
    [Test, AutoData]
    public void GetUkprn_FindsClaimAndReturnsValue(string ukprn)
    {
        ClaimsPrincipal sut = SetupClaimsPrincipal(ProviderClaims.Ukprn, ukprn);

        sut.GetUkprn().Should().Be(ukprn);
    }

    [TestCase(ServiceClaim.DAV, ServiceClaim.DAV, true)]
    [TestCase(ServiceClaim.DAC, ServiceClaim.DAV, true)]
    [TestCase(ServiceClaim.DAB, ServiceClaim.DAV, true)]
    [TestCase(ServiceClaim.DAA, ServiceClaim.DAV, true)]
    [TestCase(ServiceClaim.DAV, ServiceClaim.DAC, false)]
    [TestCase(ServiceClaim.DAC, ServiceClaim.DAC, true)]
    [TestCase(ServiceClaim.DAB, ServiceClaim.DAC, true)]
    [TestCase(ServiceClaim.DAA, ServiceClaim.DAC, true)]
    [TestCase(ServiceClaim.DAV, ServiceClaim.DAB, false)]
    [TestCase(ServiceClaim.DAC, ServiceClaim.DAB, false)]
    [TestCase(ServiceClaim.DAB, ServiceClaim.DAB, true)]
    [TestCase(ServiceClaim.DAA, ServiceClaim.DAB, true)]
    [TestCase(ServiceClaim.DAV, ServiceClaim.DAA, false)]
    [TestCase(ServiceClaim.DAC, ServiceClaim.DAA, false)]
    [TestCase(ServiceClaim.DAB, ServiceClaim.DAA, false)]
    [TestCase(ServiceClaim.DAA, ServiceClaim.DAA, true)]
    public void HasPermission_ReturnsAppropriateResponse(ServiceClaim actualClaim, ServiceClaim claimToCheck, bool expectedResult)
    {
        ClaimsPrincipal sut = SetupClaimsPrincipal(ProviderClaims.Service, actualClaim.ToString());

        sut.HasPermission(claimToCheck).Should().Be(expectedResult);
    }

    private static ClaimsPrincipal SetupClaimsPrincipal(string key, string value)
    {
        ClaimsPrincipal sut = new();
        sut.AddIdentity(new ClaimsIdentity([new Claim(key, value)]));
        return sut;
    }
}
