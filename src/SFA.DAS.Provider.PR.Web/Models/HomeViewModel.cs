using Humanizer;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Web.Models;

public class HomeViewModel : HomeSubmitModel
{
    public string TotalCount { get; }
    public IEnumerable<EmployerPermissionViewModel> Employers { get; }
    public string ClearFiltersLink { get; }

    public HomeViewModel(GetProviderRelationshipsResponse source, string clearFiltersLink)
    {
        Employers = source.Employers.Select(e => (EmployerPermissionViewModel)e);
        TotalCount = "employer".ToQuantity(source.TotalCount);
        ClearFiltersLink = clearFiltersLink;
    }
}

public class HomeSubmitModel
{
    public string? SearchTerm { get; set; }
    public bool HasPendingRequest { get; set; }

    public Dictionary<string, string> ToQueryString()
    {
        Dictionary<string, string> result = new();

        if (!string.IsNullOrWhiteSpace(SearchTerm)) result.Add(nameof(SearchTerm), SearchTerm.Trim());

        if (HasPendingRequest) result.Add(nameof(HasPendingRequest), true.ToString());

        return result;
    }
}
