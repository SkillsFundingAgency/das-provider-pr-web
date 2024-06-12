using RestEase;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Domain.Interfaces;

public interface IOuterApiClient
{
    [AllowAnyStatusCode]
    [Get("/providerAccounts/{ukprn}")]
    Task<Response<GetProviderStatusResponse>> GetProviderStatus([Path] int ukprn, CancellationToken cancellationToken);
}
