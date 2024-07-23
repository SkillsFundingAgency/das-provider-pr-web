using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Web.Models;

public class HomeViewModel
{
    public int TotalCount { get; }
    public IEnumerable<EmployerPermissionViewModel> Employers { get; }

    public HomeViewModel(GetProviderRelationshipsResponse source)
    {
        Employers = source.Employers.Select(e => (EmployerPermissionViewModel)e);
        TotalCount = source.TotalCount;
    }
}
