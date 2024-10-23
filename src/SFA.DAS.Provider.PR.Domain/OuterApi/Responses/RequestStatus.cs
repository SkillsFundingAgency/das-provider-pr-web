namespace SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
public enum RequestStatus : short
{
    New,
    Sent,
    Accepted,
    Declined,
    Expired,
    Deleted
}
