namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class AddPermissionsAndEmployerViewModel : AddPermissionsAndEmployerSubmitModel
{
    public AddPermissionsAndEmployerViewModel() { }

    public required string Email { get; set; }
    public required string CancelLink { get; set; }
    public required string LegalName { get; set; }
    public required long Ukprn { get; set; }
}

public class AddPermissionsAndEmployerSubmitModel : PermissionDescriptionsViewModel
{
}