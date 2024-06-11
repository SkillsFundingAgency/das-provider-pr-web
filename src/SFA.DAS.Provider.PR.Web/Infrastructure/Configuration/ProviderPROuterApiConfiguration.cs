using SFA.DAS.Http.Configuration;

namespace SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;

public class ProviderPROuterApiConfiguration : IApimClientConfiguration
{
    public string ApiBaseUrl { get; set; } = null!;

    public string SubscriptionKey { get; set; } = null!;

    public string ApiVersion { get; set; } = null!;
}

