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
        HomeSubmitModel sut = new() { PendingRequest = true };

        var actual = sut.ToQueryString();

        actual.Should().ContainKey(nameof(HomeSubmitModel.PendingRequest)).WhoseValue.Should().Be(true.ToString());
    }

    [Test]
    public void ToQueryString_HasPendingRequestIsFalse_DoesNotAddToDictionary()
    {
        HomeSubmitModel sut = new() { PendingRequest = false };

        var actual = sut.ToQueryString();

        actual.Should().NotContainKey(nameof(HomeSubmitModel.PendingRequest));
    }

    [TestCase(true, false, true)]
    [TestCase(false, true, true)]
    [TestCase(false, false, false)]
    [TestCase(true, true, false)]
    public void ToQueryString_ConditionallyAddsHasCreateCohortPermission(bool yesSelected, bool noSelected, bool isAdded)
    {
        HomeSubmitModel sut = new() { HasAddApprenticePermission = yesSelected, HasNotAddApprenticePermission = noSelected };

        var actual = sut.ToQueryString();

        if (isAdded)
        {
            actual.Should().ContainKey(HomeSubmitModel.HasCreateCohortPermissionKey).WhoseValue.Should().Be(yesSelected.ToString());
        }
        else
        {
            actual.Should().NotContainKey(HomeSubmitModel.HasCreateCohortPermissionKey);
        }
    }
}
