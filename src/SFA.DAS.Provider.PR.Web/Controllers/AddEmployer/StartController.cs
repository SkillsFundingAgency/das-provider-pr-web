using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer/start", Name = RouteNames.AddEmployerStart)]
public class StartController(ISessionService _sessionService) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/Start.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        _sessionService.Delete<AddEmployerSessionModel>();
        var continueLink = Url.RouteUrl(RouteNames.AddEmployerSearchByEmail, new { ukprn });
        return View(ViewPath, new StartViewModel(continueLink!, Url.RouteUrl(RouteNames.Home, new { ukprn })!));
    }
}
