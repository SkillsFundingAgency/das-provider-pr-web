namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class SearchByEmailViewModel : SearchByEmailSubmitViewModel
{
    public SearchByEmailViewModel() { }

    public required string CancelLink { get; init; }
    public required string BackLink { get; init; }
    public required long Ukprn { get; init; }

}

public class SearchByEmailSubmitViewModel
{
    public string? Email { get; set; }
}