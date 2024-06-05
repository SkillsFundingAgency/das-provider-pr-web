using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Route("/{ukprn}")]
    public IActionResult ProviderHome(int ukprn)
    {
        return View();
    }
}
