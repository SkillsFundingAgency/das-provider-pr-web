using RestEase;
using SFA.DAS.Provider.PR.Domain.OuterApi.Requests.Commands;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Domain.Interfaces;

public interface IOuterApiClient
{
    [AllowAnyStatusCode]
    [Get("/providerAccounts/{ukprn}")]
    Task<Response<GetProviderStatusResponse>> GetProviderStatus([Path] int ukprn, CancellationToken cancellationToken);

    [Get("relationships/{ukprn}")]
    Task<GetProviderRelationshipsResponse> GetProviderRelationships([Path] long ukprn, [QueryMap] IDictionary<string, string> queryString, CancellationToken cancellationToken);

    [Get("/relationships")]
    Task<GetProviderRelationshipResponse> GetProviderRelationship([Query] long ukprn, [Query] long accountLegalEntityId, CancellationToken cancellationToken);

    [Get("relationships/employeraccount")]
    Task<GetRelationshipByEmailResponse> GetRelationshipByEmail([Query] string email, [Query] long ukprn, CancellationToken cancellationToken);

    [Post("/requests/addaccount")]
    Task<AddAccountRequestCommandResponse> AddRequest([Body] AddAccountRequestCommand command, CancellationToken cancellationToken);

    [Post("/requests/createaccount")]
    Task<CreateAccountRequestCommandResponse> CreateAccount([Body] CreateAccountRequestCommand command, CancellationToken cancellationToken);

    [Post("/requests/permission")]
    Task<CreatePermissionRequestResponse> CreatePermissions([Body] CreatePermissionRequestCommand command, CancellationToken cancellationToken);

    [Get("employeraccount")]
    Task<GetRelationshipsByUkprnPayeAornResponse> GetProviderRelationshipsByUkprnPayeAorn([Query] long ukprn, [Query] string aorn, [Query] string encodedPaye, CancellationToken cancellationToken);

    [AllowAnyStatusCode]
    [Get("requests")]
    Task<Response<GetRequestByUkprnPayeResponse>> GetRequest([Query] long ukprn, [Query] string paye, CancellationToken cancellationToken);

    [AllowAnyStatusCode]
    [Get("requests")]
    Task<Response<GetRequestByUkprnEmailResponse>> GetRequestByUkprnAndEmail([Query] long ukprn, [Query] string email, CancellationToken cancellationToken);

    [AllowAnyStatusCode]
    [Get("requests/{requestid}")]
    Task<Response<GetRequestsByRequestIdResponse>> GetRequestByRequestId([Path] Guid requestid, CancellationToken cancellationToken);
}