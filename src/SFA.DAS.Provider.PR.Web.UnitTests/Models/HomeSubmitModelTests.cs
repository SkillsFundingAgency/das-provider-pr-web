using FluentAssertions;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;

public class HomeSubmitModelTests
{
    [Test]
    public void ToQueryString_SearchTermHasValue_AddsSearchTerm()
    {
        HomeSubmitModel sut = new() { SearchTerm = "search term" };

        var actual = sut.ToQueryString();

        actual.Should().ContainKey(nameof(HomeSubmitModel.SearchTerm)).WhoseValue.Should().Be(sut.SearchTerm);
    }

    [TestCase("")]
    [TestCase("  ")]
    public void ToQueryString_SearchTermIsEmptyOrWhiteSpace_DoesNotAddSearchTerm(string searchTerm)
    {
        HomeSubmitModel sut = new() { SearchTerm = searchTerm };

        var actual = sut.ToQueryString();

        actual.Should().NotContainKey(nameof(HomeSubmitModel.SearchTerm));
    }

    [Test]
    public void ToQueryString_HasPendingRequestIsTrue_AddsToDictionary()
    {
        HomeSubmitModel sut = new() { HasPendingRequest = true };

        var actual = sut.ToQueryString();

        actual.Should().ContainKey(nameof(HomeSubmitModel.HasPendingRequest)).WhoseValue.Should().Be(true.ToString());
    }

    [Test]
    public void ToQueryString_HasPendingRequestIsFalse_DoesNotAddToDictionary()
    {
        HomeSubmitModel sut = new() { HasPendingRequest = false };

        var actual = sut.ToQueryString();

        actual.Should().NotContainKey(nameof(HomeSubmitModel.HasPendingRequest));
    }
}
