using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer/invitationsent", Name = RouteNames.InvitationSent)]
public class InvitationSentController(ISessionService _sessionService) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/InvitationSent.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        _sessionService.Delete<AddEmployerSessionModel>();

        var viewEmployersAndManagePermissionsLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!;
        var viewModel = new InvitationSentViewModel(ukprn, sessionModel.Email, viewEmployersAndManagePermissionsLink);

        return View(ViewPath, viewModel);
    }
}
