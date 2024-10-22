using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Web.Models;

public class RequestPermissionsViewModel : RequestPermissionsSubmitModel
{
    public Operation[] Operations { get; set; } = [];

    public string AccountLegalEntityName { get; set; } = null!;

    public string CancelLink { get; set; } = null!;

    public string BackLink { get; set; } = null!;

    public long Ukprn { get; set; }

    public long AccountLegalEntityId { get; set; }

    public static implicit operator RequestPermissionsViewModel(GetProviderRelationshipResponse response)
    {
        return new RequestPermissionsViewModel
        {
            Operations = response.Operations,
            AccountLegalEntityName = response.AccountLegalEntityName,
            Ukprn = response.Ukprn,
            AccountLegalEntityId = response.AccountLegalEntityId
        };
    }
}

public class RequestPermissionsSubmitModel
{

}