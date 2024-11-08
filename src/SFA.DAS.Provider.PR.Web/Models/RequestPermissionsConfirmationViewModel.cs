namespace SFA.DAS.Provider.PR.Web.Models;

public class RequestPermissionsConfirmationViewModel
{
    public long Ukprn { get; set; }
    public required string AccountLegalEntityName { get; set; }
    public string EmployersLink { get; set; } = null!;
}
