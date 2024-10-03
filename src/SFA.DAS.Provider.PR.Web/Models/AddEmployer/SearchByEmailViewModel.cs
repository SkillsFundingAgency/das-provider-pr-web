namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class SearchByEmailModel : SearchByEmailSubmitModel
{
    public SearchByEmailModel() { }

    public required string CancelLink { get; init; }
    public required string BackLink { get; init; }
    public required long Ukprn { get; init; }

}

public class SearchByEmailSubmitModel
{
    public string? Email { get; set; }
}