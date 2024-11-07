namespace SFA.DAS.Provider.PR.Web.Models;

public class RequestPermissionsConfirmationViewModel
{
    public int Ukprn { get; set; }
    public string AccountLegalEntityName { get; set; }
    public string EmployersLink { get; set; } = null!;
}
