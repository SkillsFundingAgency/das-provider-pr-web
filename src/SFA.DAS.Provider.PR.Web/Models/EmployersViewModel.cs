namespace SFA.DAS.Provider.PR.Web.Models;

public class EmployersViewModel : EmployersSubmitModel
{
    public string TotalCount { get; init; } = null!;
    public IEnumerable<EmployerPermissionViewModel> Employers { get; init; } = null!;
    public string ClearFiltersLink { get; init; } = null!;
    public string AddEmployerLink { get; init; } = null!;
    public PaginationViewModel Pagination { get; init; } = null!;
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
