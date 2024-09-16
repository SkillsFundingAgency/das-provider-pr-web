namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class ContactDetailsViewModel : ContactDetailsSubmitModel
{
    public ContactDetailsViewModel() { }

    public required string CancelLink { get; init; }
    public required string BackLink { get; init; }
    public required long Ukprn { get; init; }
    public required string OrganisationName { get; init; }
    public required string Paye { get; set; }
    public required string Aorn { get; set; }
}

public class ContactDetailsSubmitModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}