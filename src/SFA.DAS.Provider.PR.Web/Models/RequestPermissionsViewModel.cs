using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Models;

public class RequestPermissionsViewModel : RequestPermissionsSubmitModel
{
    public string AccountLegalEntityName { get; set; } = null!;

    public string BackLink { get; set; } = null!;


    public static implicit operator RequestPermissionsViewModel(GetProviderRelationshipResponse response)
    {
        return new RequestPermissionsViewModel
        {
            AccountLegalEntityName = response.AccountLegalEntityName.ToUpper()
        };
    }
}

public class RequestPermissionsSubmitModel : PermissionDescriptionsViewModel
{
    public Operation[] ExistingOperations { get; set; } = [];
    public Operation[] UpdatedOperations { get; set; } = [];
}