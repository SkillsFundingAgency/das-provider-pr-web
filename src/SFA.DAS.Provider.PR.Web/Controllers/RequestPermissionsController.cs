using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;
public class RequestPermissionsController : Controller
{
    [HttpGet]
    [Route("{ukprn:int}/employers/{accountlegalentityid}/permissionsRequested", Name = RouteNames.RequestPermissionsConfirmation)]
    public async Task<IActionResult> PermissionsRequested([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        //Get session model for employer name (plus whatever is needed for the api call)
        //API call goes here
        var employerName = "TEMP NAME";

        var viewModel = new RequestPermissionsConfirmationViewModel
        {
            Ukprn = ukprn,
            AccountLegalEntityName = employerName,
            EmployersLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!
        };

        return View(viewModel);
    }
}
