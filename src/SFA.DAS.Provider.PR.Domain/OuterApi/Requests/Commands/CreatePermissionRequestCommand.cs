using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Domain.OuterApi.Requests.Commands;

public class CreatePermissionRequestCommand
{
    public required long? Ukprn { get; set; }

    public required string RequestedBy { get; set; }

    public required long AccountLegalEntityId { get; set; }

    public required List<Operation> Operations { get; set; } = [];

    public required long AccountId { get; set; }
}
