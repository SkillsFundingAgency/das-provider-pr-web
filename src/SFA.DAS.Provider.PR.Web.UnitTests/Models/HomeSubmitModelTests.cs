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

    [TestCase(true, false, true)]
    [TestCase(false, true, true)]
    [TestCase(false, false, false)]
    [TestCase(true, true, false)]
    public void ToQueryString_ConditionallyAddsHasCreateCohortPermission(bool yesSelected, bool noSelected, bool isAdded)
    {
        HomeSubmitModel sut = new() { HasAddApprenticePermission = yesSelected, HasNoAddApprenticePermission = noSelected };

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

    [TestCase(true, false, false, true)]
    [TestCase(false, true, false, true)]
    [TestCase(false, false, true, true)]
    [TestCase(true, true, true, true)]
    [TestCase(false, false, false, false)]
    public void ToQueryString_ConditionallyAddsHasCreateCohortPermission(bool yesSelected, bool yesWithReviewSelected, bool noSelected, bool isAdded)
    {
        HomeSubmitModel sut = new() { HasRecruitmentPermission = yesSelected, HasRecruitmentWithReviewPermission = yesWithReviewSelected, HasNoRecruitmentPermission = noSelected };

        var actual = sut.ToQueryString();

        if (isAdded)
        {
            actual.Should().ContainKey(nameof(HomeSubmitModel.HasRecruitmentPermission)).WhoseValue.Should().Be(yesSelected.ToString());
            actual.Should().ContainKey(nameof(HomeSubmitModel.HasRecruitmentWithReviewPermission)).WhoseValue.Should().Be(yesWithReviewSelected.ToString());
            actual.Should().ContainKey(nameof(HomeSubmitModel.HasNoRecruitmentPermission)).WhoseValue.Should().Be(noSelected.ToString());
        }
        else
        {
            actual.Should().NotContainKey(nameof(HomeSubmitModel.HasRecruitmentPermission));
            actual.Should().NotContainKey(nameof(HomeSubmitModel.HasRecruitmentWithReviewPermission));
            actual.Should().NotContainKey(nameof(HomeSubmitModel.HasNoRecruitmentPermission));
        }
    }
}
