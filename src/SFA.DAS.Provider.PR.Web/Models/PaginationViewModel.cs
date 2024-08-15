namespace SFA.DAS.Provider.PR.Web.Models;

public class PaginationViewModel
{
    public const string PreviousPageTitle = "« Previous";
    public const string NextPageTitle = "Next »";

    public List<PageLink> Pages { get; } = [];

    public PaginationViewModel(int currentPage, int totalCount, int pageSize)
    {
        if (totalCount == 0) return;
        var totalPages = Convert.ToInt32(Math.Ceiling((double)totalCount / pageSize));
        if (currentPage > totalPages) return;
        var (startPage, endPage) = GetPageRange(currentPage, totalCount, pageSize);
        AddPreviousLinkIfApplicable(totalPages, startPage);
        AddPageLinks(currentPage, startPage, endPage);
        AddNextLinkIfApplicable(totalPages, endPage);
    }

    private void AddNextLinkIfApplicable(int totalPages, int endPage)
    {
        if (endPage < totalPages) Pages.Add(new(NextPageTitle, GetPageLink(endPage + 1)));
    }

    private void AddPageLinks(int currentPage, int startPage, int endPage)
    {
        for (int pageNumber = startPage; pageNumber <= endPage; pageNumber++)
        {
            string? pageUrl = pageNumber == currentPage ? null : GetPageLink(pageNumber);
            Pages.Add(new(pageNumber.ToString(), pageUrl));
        }
    }

    private void AddPreviousLinkIfApplicable(int totalPages, int startPage)
    {
        if (startPage > 1 && totalPages > 6) Pages.Add(new(PreviousPageTitle, GetPageLink(startPage - 1)));
    }

    private static string? GetPageLink(int pageNumber)
    {
        return pageNumber.ToString();
    }

    static (int startPage, int endPage) GetPageRange(int currentPage, int totalRecords, int pageSize)
    {
        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        int startPage = Math.Max(1, currentPage - 2);
        int endPage = Math.Min(totalPages, currentPage + 3);

        if (endPage - startPage < 5)
        {
            if (startPage == 1)
            {
                endPage = Math.Min(totalPages, startPage + 5);
            }
            else if (endPage == totalPages)
            {
                startPage = Math.Max(1, endPage - 5);
            }
        }

        if ((totalPages > 6) && (endPage - startPage < 5))
        {
            if (startPage == 1)
            {
                endPage = startPage + 5;
            }
            else if (endPage == totalPages)
            {
                startPage = endPage - 5;
            }
        }


        return (startPage, endPage);
    }
}

public record PageLink(string Title, string? Url);
