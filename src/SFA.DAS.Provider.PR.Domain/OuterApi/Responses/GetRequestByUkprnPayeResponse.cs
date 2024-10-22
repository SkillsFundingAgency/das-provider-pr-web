namespace SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

public class GetRequestByUkprnPayeResponse
{
    public Guid RequestId { get; set; }
    public string? EmployerOrganisationName { get; set; }
    public string? RequestType { get; set; }
    public long? AccountLegalEntityId { get; set; }
}