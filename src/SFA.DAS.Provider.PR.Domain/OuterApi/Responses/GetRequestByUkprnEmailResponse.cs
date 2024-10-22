namespace SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

public class GetRequestByUkprnEmailResponse
{
    public Guid RequestId { get; set; }
    public string? EmployerOrganisationName { get; set; }
    public string? RequestType { get; set; }
    public long? AccountLegalEntityId { get; set; }
}