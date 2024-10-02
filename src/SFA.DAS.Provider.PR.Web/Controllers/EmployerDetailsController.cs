using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("")]
public class EmployerDetailsController(IOuterApiClient _outerApiclient) : Controller
{
    [Route("{ukprn:int}/employers/{agreementid:string}", Name = RouteNames.EmployerDetails)]
    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromQuery] EmployerPermissionViewModel employer,
        CancellationToken cancellationToken)
    {
        GetProviderRelationshipResponse response =
            await _outerApiclient.GetProviderRelationship(ukprn, (long)employer.AccountLegalEntityId!, cancellationToken);

        EmployerDetailsViewModel model = response;

        model.EmployersLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!;

        return View(model);
    }
}
