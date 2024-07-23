using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer/start", Name = RouteNames.AddEmployerStart)]
public class StartController : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/Start.cshtml";

    [HttpGet]
    public IActionResult Index()
    {
        return View(ViewPath, new StartViewModel("#", "#"));
    }
}
