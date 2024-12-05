using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Domain.OuterApi.Requests.Commands;
public class AddAccountRequestCommand
{
    public AddAccountRequestCommand() { }

    public required long AccountId { get; set; }

    public required long? Ukprn { get; set; }

    public required string RequestedBy { get; set; }

    public required long AccountLegalEntityId { get; set; }

    public string? EmployerContactEmail { get; set; }
    public string? Paye { get; set; }

    public required List<Operation> Operations { get; set; } = [];
}