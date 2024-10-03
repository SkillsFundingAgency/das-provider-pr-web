namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class SearchByPayeModel : SearchByPayeSubmitModel
{
    public SearchByPayeModel() { }

    public required string CancelLink { get; init; }
    public required string BackLink { get; init; }
    public required long Ukprn { get; init; }
}

public class SearchByPayeSubmitModel
{
    public string? Email { get; set; }
    public string? Paye { get; set; }
    public string? Aorn { get; set; }
}