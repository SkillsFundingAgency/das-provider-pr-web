namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class SearchByPayeViewModel : SearchByPayeSubmitViewModel
{
    public SearchByPayeViewModel() { }

    public required string CancelLink { get; init; }
    public required string BackLink { get; init; }
    public required long Ukprn { get; init; }
}

public class SearchByPayeSubmitViewModel
{
    public string? Email { get; set; }
    public string? Paye { get; set; }
    public string? Aorn { get; set; }
}