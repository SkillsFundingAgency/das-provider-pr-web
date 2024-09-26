using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Domain.OuterApi.Requests.Commands;

public class CreateAccountRequestCommand
{
    public CreateAccountRequestCommand() { }


    public required long? Ukprn { get; set; }

    public required string RequestedBy { get; set; }
    public required string EmployerOrganisationName { get; set; }
    public required string EmployerContactFirstName { get; set; }
    public required string EmployerContactLastName { get; set; }
    public required string EmployerContactEmail { get; set; }
    public required string EmployerPaye { get; set; }
    public required string EmployerAorn { get; set; } 

    public required List<Operation> Operations { get; set; } = [];
}