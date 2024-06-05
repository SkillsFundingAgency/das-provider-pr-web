using RestEase;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;

namespace SFA.DAS.Provider.PR.Domain.Interfaces;

public interface IOuterApiClient
{
    [Get("/ping")]
    Task<HttpResponseMessage> Ping();

    [Get("/provideraccounts/{ukprn}")]
    Task<GetProviderStatusResponse> GetProviderStatus([Path] int ukprn, CancellationToken cancellationToken);
}
