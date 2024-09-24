namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public class CheckDetailsViewModel
{
    public required int Ukprn { get; set; }
    public required string OrganisationName { get; set; }
    public required string Paye { get; set; }
    public required string Aorn { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PermissionToAddCohorts { get; set; }
    public required string PermissionToAddCohortsText { get; set; }
    public required string PermissionToRecruit { get; set; }
    public required string PermissionToRecruitText { get; set; }
    public required string ChangeEmployerNameLink { get; set; }
    public required string ChangePermissionsLink { get; set; }
    public required string CancelLink { get; set; }
};
