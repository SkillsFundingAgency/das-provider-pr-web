using DnsClient;
using FluentAssertions;
using SFA.DAS.Provider.PR.Application.Services;

namespace SFA.DAS.Provider.PR.Application.UnitTests.Services;

public class EmailCheckingServiceTests
{
    [TestCase("")]
    [TestCase(null)]
    [TestCase("aaaa@")]
    public async Task Email_IsValidDomain_ReturnsFalse_WithoutDnsLookup(string? email)
    {
        var isEmailValid = await EmailCheckingService.IsValidDomain(email);
        isEmailValid.Should().BeFalse();
    }

    [TestCase("xxxxxx", false)]
    [TestCase("@aaaa", false)]
    [TestCase("aaaa@NonExistentDomain50c2413d-e8e4-4330-9859-222567ad0f64.co.uk", false)]
    [TestCase("aaaa@google.com", true)]
    [TestCase("aaaa@btconnect.com", true)]
    [TestCase("aaaa@cplumbinguk.co.uk", true)]
    public async Task Email_IsValidDomain_ReturnedExpected_DnsDependent(string? email, bool isValid)
    {
        try
        {
            var isEmailValid = await EmailCheckingService.IsValidDomain(email);
            isEmailValid.Should().Be(isValid);
        }
        catch (DnsResponseException)
        {
            Assert.Ignore("DNS lookup timed out or transiently failed in test environment.");
        }
        catch (OperationCanceledException)
        {
            Assert.Ignore("DNS lookup was canceled in test environment.");
        }
    }
}
