namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class AddPermissionsAndEmployerViewModel : AddPermissionsAndEmployerSubmitViewModel
{
    public AddPermissionsAndEmployerViewModel() { }

    public required string Email { get; set; }
    public required string CancelLink { get; set; }
    public required string LegalName { get; set; }
    public required long Ukprn { get; set; }
}

public class AddPermissionsAndEmployerSubmitViewModel : PermissionDescriptionsViewModel
{
}