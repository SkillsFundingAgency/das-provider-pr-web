using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;

public class PaginationViewModelTests
{
    private const int PageSize = 10;
    [Test]
    [InlineAutoData(1, 0, 10, "When no records")]
    [InlineAutoData(3, 10, 10, "When page number is greater than total pages")]
    public void ReturnsEmptyModel(int currentPage, int totalCount, int pageSize, string testMessage)
    {
        var sut = new PaginationViewModel(currentPage, totalCount, pageSize);
        sut.Pages.Count.Should().Be(0, testMessage);
    }

    [Test]
    [InlineAutoData(1, 10, false, "When one page")]
    [InlineAutoData(6, 60, false, "When total pages are less than equal to 6")]
    [InlineAutoData(3, 100, false, "When current page is equal to 3")]
    [InlineAutoData(1, 0, false, "When no records")]
    [InlineAutoData(4, 70, true, "When two pages previous to current page is greater than 1")]
    public void AddsPreviousLink(int currentPage, int totalCount, bool expected, string testMessage)
    {
        var actual = new PaginationViewModel(currentPage, totalCount, PageSize);

        if (expected)
        {
            actual.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
        }
        else
        {
            actual.Pages.Find(p => p.Title == PaginationViewModel.PreviousPageTitle).Should().BeNull();
        }
    }

    [Test]
    [InlineAutoData(1, 0, false, "When no records")]
    [InlineAutoData(1, 10, false, "When one page")]
    [InlineAutoData(7, 100, false, "When on fourth to last page")]
    [InlineAutoData(8, 100, false, "When on third to last page")]
    [InlineAutoData(9, 100, false, "When on second to last page")]
    [InlineAutoData(10, 100, false, "When on last page")]
    [InlineAutoData(6, 100, true, "When the number of total pages is beyond display range")]
    public void AddsNextLink(int currentPage, int totalCount, bool expected, string testMessage)
    {
        var actual = new PaginationViewModel(currentPage, totalCount, PageSize);

        if (expected)
        {
            actual.Pages[actual.Pages.Count - 1].Title.Should().Be(PaginationViewModel.NextPageTitle);
        }
        else
        {
            actual.Pages.Find(p => p.Title == PaginationViewModel.NextPageTitle).Should().BeNull();
        }
    }

    [TestCase(1, 1, 1, "1", "1", "When there is only 1 page")]
    [TestCase(1, 60, 6, "1", "6", "When current page is 1 and there are 6 pages")]
    [TestCase(6, 60, 6, "1", "6", "When total pages are less than maximum page links")]
    [TestCase(1, 70, 7, "1", PaginationViewModel.NextPageTitle, "When current page is 1 and total pages are greater than maximum page links")]
    [TestCase(3, 70, 7, "1", PaginationViewModel.NextPageTitle, "When current page and first page are both in range and there are more pages")]
    [TestCase(4, 70, 7, PaginationViewModel.PreviousPageTitle, "7", "When current page and first page are both in range and there are previous pages")]
    [TestCase(7, 70, 7, PaginationViewModel.PreviousPageTitle, "7", "When current page is the last page in range and there are previous pages")]
    [TestCase(4, 100, 8, PaginationViewModel.PreviousPageTitle, PaginationViewModel.NextPageTitle, "When there are pages before and after the range")]
    public void CalculatesCorrectStartPage(int currentPage, int totalCount, int expectedPageLinksCount, string expectedStartPage, string expectedEndPage, string testMessage)
    {
        var model = new PaginationViewModel(currentPage, totalCount, PageSize);

        using (new AssertionScope(testMessage))
        {
            model.Pages.Count.Should().Be(expectedPageLinksCount, $"Expected page count: {expectedPageLinksCount}");
            model.Pages[0].Title.Should().Be(expectedStartPage, $"Expected start page: {expectedStartPage}");
            model.Pages[model.Pages.Count - 1].Title.Should().Be(expectedEndPage, $"Expected end page: {expectedEndPage}");
        }
    }
}
