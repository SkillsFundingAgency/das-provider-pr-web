using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;
public class RequestPermissionsController : Controller
{
    [Route("{ukprn:int}/employers/{accountlegalentityid}/requestpermissions", Name = RouteNames.RequestPermissions)]
    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn, [FromRoute] string accountlegalentityid, [FromQuery] RequestPermissionsViewModel model)
    {
        model.CancelLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!;
        model.BackLink = Url.RouteUrl(RouteNames.EmployerDetails, new { ukprn, accountlegalentityid })!;
        model.AccountLegalEntityId = accountlegalentityid;
        model.Ukprn = ukprn;

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(RequestPermissionsViewModel model)
    {
        return RedirectToAction("Index", new { model.Ukprn, model.AccountLegalEntityId });
    }
}
