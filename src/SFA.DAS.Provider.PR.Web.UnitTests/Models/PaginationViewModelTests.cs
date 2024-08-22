using AutoFixture.NUnit3;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR_Web.UnitTests.TestHelpers;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Models;

public class PaginationViewModelTests
{
    private const int PageSize = 10;
    [Test]
    [InlineAutoData(1, 0, "When no records")]
    [InlineAutoData(1, 10, "When only enough records for one page")]
    [InlineAutoData(3, 10, "When page number is greater than total pages")]
    public void ReturnsEmptyModel(int currentPage, int totalCount, string testMessage)
    {
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
        Dictionary<string, object> queryParams = new() { { nameof(EmployersSubmitModel.PageNumber), currentPage } };
        var sut = new PaginationViewModel(totalCount, PageSize, urlHelperMock.Object, RouteNames.Employers, queryParams);
        sut.Pages.Count.Should().Be(0, testMessage);
    }

    [Test]
    [InlineAutoData(1, 0, 0, "Previous link should not exist when there are no records")]
    [InlineAutoData(1, 10, 0, "Previous link should not exist when one page only")]
    [InlineAutoData(1, 70, 0, "Previous link should not exist when on first page with more pages")]
    [InlineAutoData(2, 20, 1, "Previous link should point to previous to current page when current page > 1 and total pages > 1")]
    [InlineAutoData(7, 70, 6, "Previous link should point to previous to current page when on last page and total pages > 1")]
    public void AddsPreviousLink(int currentPage, int totalCount, int expectedPageNumberInPreviousLink, string testMessage)
    {
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
        AddUrlForRoute(urlHelperMock, expectedPageNumberInPreviousLink);
        Dictionary<string, object> queryParams = new() { { nameof(EmployersSubmitModel.PageNumber), currentPage.ToString() } };
        var sut = new PaginationViewModel(totalCount, PageSize, urlHelperMock.Object, RouteNames.Employers, queryParams);

        if (expectedPageNumberInPreviousLink > 0)
        {
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].Url.Should().Contain($"PageNumber={expectedPageNumberInPreviousLink}");
        }
        else
        {
            sut.Pages.Should().NotContain(p => p.Title == PaginationViewModel.PreviousPageTitle);
        }
    }

    private static void AddUrlForRoute(Mock<IUrlHelper> urlHelperMock, int expectedPageNumber)
    {
        urlHelperMock
           .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c => c.RouteName!.Equals(RouteNames.Employers) && ((Dictionary<string, object>)c.Values!)["PageNumber"].Equals(expectedPageNumber))))
           .Returns($"{TestConstants.DefaultUrl}?PageNumber={expectedPageNumber}");
    }

    [Test]
    [InlineAutoData(1, 0, 0, "Next link should not exist when no records")]
    [InlineAutoData(1, 10, 0, "Next link should not exist when one page only")]
    [InlineAutoData(2, 20, 0, "Next link should not exist when on last page")]
    [InlineAutoData(1, 20, 2, "Next link should point to current page + 1 when current page is less than total pages")]
    public void AddsNextLink(int currentPage, int totalCount, int expectedPageNumberInTheNextLink, string testMessage)
    {
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
        AddUrlForRoute(urlHelperMock, expectedPageNumberInTheNextLink);
        Dictionary<string, object> queryParams = new() { { nameof(EmployersSubmitModel.PageNumber), currentPage.ToString() } };
        var sut = new PaginationViewModel(totalCount, PageSize, urlHelperMock.Object, RouteNames.Employers, queryParams);

        if (expectedPageNumberInTheNextLink > 0)
        {
            var lastPage = sut.Pages.Count - 1;
            sut.Pages[lastPage].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[lastPage].Url.Should().Contain($"PageNumber={expectedPageNumberInTheNextLink}");
        }
        else
        {
            sut.Pages.Find(p => p.Title == PaginationViewModel.NextPageTitle).Should().BeNull();
        }
    }

    [Test]
    public void SetsCorrectPageLinks()
    {
        int currentPage = 2;
        int totalCount = 30;
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
        AddUrlForRoute(urlHelperMock, 1);
        AddUrlForRoute(urlHelperMock, 2);
        AddUrlForRoute(urlHelperMock, 3);

        Dictionary<string, object> queryParams = new() { { nameof(EmployersSubmitModel.PageNumber), currentPage.ToString() } };
        var sut = new PaginationViewModel(totalCount, PageSize, urlHelperMock.Object, RouteNames.Employers, queryParams);

        using (new AssertionScope())
        {
            sut.Pages.Should().HaveCount(5);
            sut.Pages[0].Title.Should().Be(PaginationViewModel.PreviousPageTitle);
            sut.Pages[0].Url.Should().Contain($"PageNumber={1}");
            sut.Pages[1].Title.Should().Be("1");
            sut.Pages[1].Url.Should().Contain($"PageNumber={1}");
            sut.Pages[2].Title.Should().Be("2");
            sut.Pages[2].Url.Should().BeNull();
            sut.Pages[3].Title.Should().Be("3");
            sut.Pages[3].Url.Should().Contain($"PageNumber={3}");
            sut.Pages[4].Title.Should().Be(PaginationViewModel.NextPageTitle);
            sut.Pages[4].Url.Should().Contain($"PageNumber={3}");
        }
    }


    [TestCase(1, 10, 0, 0, "When there is only 1 page")]
    [TestCase(2, 11, 2, 1, "When there is only 1 page")]
    [TestCase(3, 70, 6, 1, "When there is only 1 page")]
    [TestCase(4, 70, 6, 2, "When there is only 1 page")]
    [TestCase(9, 90, 6, 4, "When there is only 1 page")]
    public void CreatesCorrectNumberOfPageLinks(int currentPage, int totalCount, int expectedPageLinksCount, int startPageNumber, string testMessage)
    {
        Mock<IUrlHelper> urlHelperMock = new Mock<IUrlHelper>();
        Dictionary<string, object> queryParams = new() { { nameof(EmployersSubmitModel.PageNumber), currentPage.ToString() } };
        var sut = new PaginationViewModel(totalCount, PageSize, urlHelperMock.Object, RouteNames.Employers, queryParams);

        sut.Pages.RemoveAll(p => p.Title == PaginationViewModel.PreviousPageTitle || p.Title == PaginationViewModel.NextPageTitle);

        using (new AssertionScope(testMessage))
        {
            sut.Pages.Should().HaveCount(expectedPageLinksCount, $"Expected page links count: {expectedPageLinksCount}");
            if (expectedPageLinksCount > 0)
            {
                var pageNumber = startPageNumber;
                for (int index = 0; index < expectedPageLinksCount; index++)
                {
                    sut.Pages[index].Title.Should().Be(pageNumber.ToString(), $"Expected page title at index:{index}");
                    pageNumber++;
                }
            }
        }
    }
}
