using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.PR.Web.Authorization;

namespace SFA.DAS.Provider.PR.Web.AppStart;

[ExcludeFromCodeCoverage]
public class ProviderStubAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string Ukprn = "10000001";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProviderStubAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IHttpContextAccessor httpContextAccessor) : base(options, logger, encoder)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, Ukprn),
            new Claim(ProviderClaims.DisplayName, "Provider User"),
            new Claim(ProviderClaims.Service, "DAA"),
            new Claim(ProviderClaims.Ukprn, Ukprn)
        };
        var identity = new ClaimsIdentity(claims, "Provider-stub");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Provider-stub");

        var result = AuthenticateResult.Success(ticket);

        _httpContextAccessor.HttpContext!.Items.Add(ClaimsIdentity.DefaultNameClaimType, Ukprn);
        _httpContextAccessor.HttpContext!.Items.Add(ProviderClaims.DisplayName, "Provider UserName");

        return Task.FromResult(result);
    }
}
