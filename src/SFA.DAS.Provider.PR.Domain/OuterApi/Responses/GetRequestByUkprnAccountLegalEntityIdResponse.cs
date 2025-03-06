namespace SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
public class GetRequestByUkprnAccountLegalEntityIdResponse
{
    public Guid? RequestId { get; set; }

    public RequestStatus? Status { get; set; }

    public RequestType RequestType { get; set; }
}
