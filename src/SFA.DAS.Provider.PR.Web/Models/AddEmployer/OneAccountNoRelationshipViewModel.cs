namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class OneAccountNoRelationshipViewModel
{
    public required string CancelLink { get; init; }
    public required string BackLink { get; init; }

    public required string ContinueLink { get; init; }
    public required long Ukprn { get; init; }
    public required string? Email { get; init; }
}
