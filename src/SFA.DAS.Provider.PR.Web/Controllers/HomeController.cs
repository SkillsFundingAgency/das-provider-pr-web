using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[ExcludeFromCodeCoverage]
[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
public class HomeController : Controller
{
    [Route("/{ukprn}")]
    [Route("", Name = RouteNames.Home)]
    public IActionResult Index()
    {
        return View();
    }
}
