using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Models.Session;

public class AddEmployerSessionModel : PermissionDescriptionsViewModel
{
    public required string Email { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string? AccountLegalEntityName { get; set; }
    public long? AccountId { get; set; }
    public string? Paye { get; set; }
    public string? Aorn { get; set; }
    public string? OrganisationName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsCheckDetailsVisited { get; set; }
    public Guid? RequestId { get; set; }
}
