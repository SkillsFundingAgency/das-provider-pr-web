using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Extensions;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
public class HomeController(IOuterApiClient _outerApiclient) : Controller
{
    public IActionResult Index()
    {
        var ukprn = User.GetUkprn();

        return RedirectToRoute(RouteNames.Home, new { ukprn });
    }

    [Route("/{ukprn:int}", Name = RouteNames.Home)]
    public async Task<IActionResult> Index(int ukprn, CancellationToken cancellationToken)
    {
        GetProviderRelationshipsResponse response = await _outerApiclient.GetProviderRelationships(ukprn, new Dictionary<string, string>(), cancellationToken);
        return View(new HomeViewModel(response));
    }

}
