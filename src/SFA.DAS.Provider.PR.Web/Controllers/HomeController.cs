using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
public class HomeController : Controller
{
    [Route("/{ukprn}")]
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
}
