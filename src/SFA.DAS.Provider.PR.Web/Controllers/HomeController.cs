using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Provider.PR.Web.Controllers;

//[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
