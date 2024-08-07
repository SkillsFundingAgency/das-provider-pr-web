using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Models.Session;

public class AddEmployerSessionModel : PermissionDescriptionsViewModel
{
    public required string Email { get; set; }
    public long? AccountLegalEntityId { get; set; }
    public string? AccountLegalEntityName { get; set; }
    public long? AccountId { get; set; }
}
