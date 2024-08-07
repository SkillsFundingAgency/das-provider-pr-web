using RestEase;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Domain.Interfaces;

public interface IOuterApiClient
{
    [AllowAnyStatusCode]
    [Get("/providerAccounts/{ukprn}")]
    Task<Response<GetProviderStatusResponse>> GetProviderStatus([Path] int ukprn, CancellationToken cancellationToken);

    [Get("relationships/{ukprn}")]
    Task<GetProviderRelationshipsResponse> GetProviderRelationships([Path] long ukprn, [QueryMap] IDictionary<string, string> queryString, CancellationToken cancellationToken);

    [Get("relationships/employeraccount/email/{email}")]
    Task<GetRelationshipByEmailResponse> GetRelationshipByEmail([Path] string email, [Query] long ukprn, CancellationToken cancellationToken);

}
