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
}
