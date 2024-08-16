using Humanizer;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Infrastructure;

namespace SFA.DAS.Provider.PR.Web.Models;

public class EmployersViewModel : EmployersSubmitModel
{
    public string TotalCount { get; }
    public IEnumerable<EmployerPermissionViewModel> Employers { get; }
    public string ClearFiltersLink { get; }
    public string AddEmployerLink { get; }
    public PaginationViewModel Pagination { get; set; }

    public EmployersViewModel(GetProviderRelationshipsResponse source, IUrlHelper urlHelper, int ukprn, int pageSize, Dictionary<string, string> queryParams)
    {
        Employers = source.Employers.Select(e => (EmployerPermissionViewModel)e);
        TotalCount = "employer".ToQuantity(source.TotalCount);
        ClearFiltersLink = urlHelper.RouteUrl(RouteNames.Employers, new { ukprn })!;
        AddEmployerLink = urlHelper.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!;
        queryParams[nameof(ukprn)] = ukprn.ToString();
        Pagination = new(source.TotalCount, pageSize, urlHelper, RouteNames.Employers, queryParams);
        PageSize = pageSize;
    }
}

public class EmployersSubmitModel
{
    public const string HasCreateCohortPermissionKey = "HasCreateCohortPermission";

    public string? SearchTerm { get; set; }
    public bool HasPendingRequest { get; set; }
    public bool HasAddApprenticePermission { get; set; }
    public bool HasNoAddApprenticePermission { get; set; }
    public bool HasRecruitmentPermission { get; set; }
    public bool HasRecruitmentWithReviewPermission { get; set; }
    public bool HasNoRecruitmentPermission { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; protected set; }

    public Dictionary<string, string> ToQueryString()
    {
        Dictionary<string, string> result = new();

        if (!string.IsNullOrWhiteSpace(SearchTerm)) result.Add(nameof(SearchTerm), SearchTerm.Trim());

        if (HasPendingRequest) result.Add(nameof(HasPendingRequest), true.ToString());

        if (HasNoAddApprenticePermission != HasAddApprenticePermission) result.Add(HasCreateCohortPermissionKey, HasAddApprenticePermission.ToString());

        if (HasRecruitmentPermission || HasRecruitmentWithReviewPermission || HasNoRecruitmentPermission)
        {
            result.Add(nameof(HasRecruitmentPermission), HasRecruitmentPermission.ToString());
            result.Add(nameof(HasRecruitmentWithReviewPermission), HasRecruitmentWithReviewPermission.ToString());
            result.Add(nameof(HasNoRecruitmentPermission), HasNoRecruitmentPermission.ToString());
        }
        result.Add(nameof(PageNumber), PageNumber.ToString());
        return result;
    }
}
