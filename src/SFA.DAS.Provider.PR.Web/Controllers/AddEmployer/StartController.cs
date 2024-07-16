using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]

[Route("/{ukprn}/addEmployer/start")]
public class StartController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new StartViewModel("#", "#"));
    }
}
