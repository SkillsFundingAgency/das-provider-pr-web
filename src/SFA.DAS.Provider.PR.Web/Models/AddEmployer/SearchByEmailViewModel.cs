namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class SearchByEmailViewModel : SearchByEmailSubmitViewModel
{
    public string CancelLink { get; }
    public string BackLink { get; }
    public long Ukprn { get; set; }

    public SearchByEmailViewModel(string cancelLink, string backLink, int ukprn)
    {
        CancelLink = cancelLink;
        BackLink = backLink;
        Ukprn = ukprn;
    }
}

public class SearchByEmailSubmitViewModel
{
    public string? Email { get; set; }
}