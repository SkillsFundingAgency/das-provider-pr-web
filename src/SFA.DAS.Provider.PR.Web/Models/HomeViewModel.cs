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
    public const string HasCreateCohortPermissionKey = "HasCreateCohortPermission";

    public string? SearchTerm { get; set; }
    public bool PendingRequest { get; set; }
    public bool HasAddApprenticePermission { get; set; }
    public bool HasNotAddApprenticePermission { get; set; }

    public Dictionary<string, string> ToQueryString()
    {
        Dictionary<string, string> result = new();

        if (!string.IsNullOrWhiteSpace(SearchTerm)) result.Add(nameof(SearchTerm), SearchTerm.Trim());

        if (PendingRequest) result.Add(nameof(PendingRequest), true.ToString());

        if (HasNotAddApprenticePermission != HasAddApprenticePermission) result.Add(HasCreateCohortPermissionKey, HasAddApprenticePermission.ToString());

        return result;
    }
}
