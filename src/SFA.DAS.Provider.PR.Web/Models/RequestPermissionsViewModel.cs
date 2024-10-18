using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Web.Models;

public class RequestPermissionsViewModel
{
    public Operation[] Operations { get; set; } = [];

    public string AccountLegalEntityName { get; set; } = null!;

    public string CancelLink { get; set; } = null!;

    public string BackLink { get; set; } = null!;

    public int Ukprn { get; set; }

    public string AccountLegalEntityId { get; set; } = null!;
}
