namespace SFA.DAS.Provider.PR.Web.Models.AddEmployer;

public record CheckDetailsViewModel(int Ukprn, string OrganisationName, string Paye, string Aorn, string Email,
    string FirstName, string LastName,
    string PermissionToAddCohorts, string PermissionToAddCohortsText,
    string PermissionToRecruit, string PerimissionToRecruitText,
    string? ChangeEmployerNameLink, string? ChangePermissionsLink, string CancelLink);
