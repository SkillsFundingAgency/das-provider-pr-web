using FluentAssertions;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;

public class EmployersSubmitModelTests
{
    [Test]
    public void ToQueryString_SearchTermHasValue_AddsSearchTerm()
    {
        EmployersSubmitModel sut = new() { SearchTerm = "search term" };

        var actual = sut.ToQueryString();

        actual.Should().ContainKey(nameof(EmployersSubmitModel.SearchTerm)).WhoseValue.Should().Be(sut.SearchTerm);
    }

    [TestCase("")]
    [TestCase("  ")]
    public void ToQueryString_SearchTermIsEmptyOrWhiteSpace_DoesNotAddSearchTerm(string searchTerm)
    {
        EmployersSubmitModel sut = new() { SearchTerm = searchTerm };

        var actual = sut.ToQueryString();

        actual.Should().NotContainKey(nameof(EmployersSubmitModel.SearchTerm));
    }

    [Test]
    public void ToQueryString_HasPendingRequestIsTrue_AddsToDictionary()
    {
        EmployersSubmitModel sut = new() { HasPendingRequest = true };

        var actual = sut.ToQueryString();

        actual.Should().ContainKey(nameof(EmployersSubmitModel.HasPendingRequest)).WhoseValue.Should().Be(true.ToString());
    }

    [Test]
    public void ToQueryString_HasPendingRequestIsFalse_DoesNotAddToDictionary()
    {
        EmployersSubmitModel sut = new() { HasPendingRequest = false };

        var actual = sut.ToQueryString();

        actual.Should().NotContainKey(nameof(EmployersSubmitModel.HasPendingRequest));
    }

    [TestCase(true, false, true)]
    [TestCase(false, true, true)]
    [TestCase(false, false, false)]
    [TestCase(true, true, false)]
    public void ToQueryString_ConditionallyAddsHasCreateCohortPermission(bool yesSelected, bool noSelected, bool isAdded)
    {
        EmployersSubmitModel sut = new() { HasAddApprenticePermission = yesSelected, HasNoAddApprenticePermission = noSelected };

        var actual = sut.ToQueryString();

        if (isAdded)
        {
            actual.Should().ContainKey(EmployersSubmitModel.HasCreateCohortPermissionKey).WhoseValue.Should().Be(yesSelected.ToString());
        }
        else
        {
            actual.Should().NotContainKey(EmployersSubmitModel.HasCreateCohortPermissionKey);
        }
    }

    [TestCase(true, false, false, true)]
    [TestCase(false, true, false, true)]
    [TestCase(false, false, true, true)]
    [TestCase(true, true, true, true)]
    [TestCase(false, false, false, false)]
    public void ToQueryString_ConditionallyAddsHasCreateCohortPermission(bool yesSelected, bool yesWithReviewSelected, bool noSelected, bool isAdded)
    {
        EmployersSubmitModel sut = new() { HasRecruitmentPermission = yesSelected, HasRecruitmentWithReviewPermission = yesWithReviewSelected, HasNoRecruitmentPermission = noSelected };

        var actual = sut.ToQueryString();

        if (isAdded)
        {
            actual.Should().ContainKey(nameof(EmployersSubmitModel.HasRecruitmentPermission)).WhoseValue.Should().Be(yesSelected.ToString());
            actual.Should().ContainKey(nameof(EmployersSubmitModel.HasRecruitmentWithReviewPermission)).WhoseValue.Should().Be(yesWithReviewSelected.ToString());
            actual.Should().ContainKey(nameof(EmployersSubmitModel.HasNoRecruitmentPermission)).WhoseValue.Should().Be(noSelected.ToString());
        }
        else
        {
            actual.Should().NotContainKey(nameof(EmployersSubmitModel.HasRecruitmentPermission));
            actual.Should().NotContainKey(nameof(EmployersSubmitModel.HasRecruitmentWithReviewPermission));
            actual.Should().NotContainKey(nameof(EmployersSubmitModel.HasNoRecruitmentPermission));
        }
    }
}
