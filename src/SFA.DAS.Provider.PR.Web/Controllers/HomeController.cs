using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Extensions;
using SFA.DAS.Provider.PR.Web.Infrastructure;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
public class HomeController() : Controller
{
    [HttpGet]
    [Route("")]
    [Route("/{ukprn}", Name = RouteNames.Home)]
    public IActionResult Index()
    {
        var ukprn = User.GetUkprn();

        return RedirectToRoute(RouteNames.Employers, new { ukprn });
    }
}
