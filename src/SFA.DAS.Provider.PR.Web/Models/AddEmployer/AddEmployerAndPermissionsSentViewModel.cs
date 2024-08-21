namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class AddEmployerAndPermissionsSentViewModel
{
    public AddEmployerAndPermissionsSentViewModel() { }

    public required string Email { get; set; }
    public required string ViewEmployersLink { get; set; }
    public required string LegalName { get; set; }
    public required long Ukprn { get; set; }
}