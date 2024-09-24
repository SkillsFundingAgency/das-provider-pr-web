namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class ChangePermissionsViewModel : ChangePermissionsSubmitModel
{
    public ChangePermissionsViewModel() { }

    public required string Email { get; set; }
    public required string BackLink { get; set; }
    public required string CancelLink { get; set; }
    public required string OrganisationName { get; set; }
    public required long Ukprn { get; set; }
}

public class ChangePermissionsSubmitModel : PermissionDescriptionsViewModel
{
}