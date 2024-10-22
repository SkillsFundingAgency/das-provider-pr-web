using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;
public class RequestPermissionsController(IOuterApiClient _outerApiclient, IEncodingService encodingService) : Controller
{
    [Route("{ukprn:int}/employers/{accountlegalentityid}/requestpermissions", Name = RouteNames.RequestPermissions)]
    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromRoute] string accountlegalentityid,
        CancellationToken cancellationToken)
    {
        var accountLegalEntityIdDecoded = encodingService.Decode(accountlegalentityid, EncodingType.PublicAccountLegalEntityId);

        GetProviderRelationshipResponse response =
            await _outerApiclient.GetProviderRelationship(ukprn, accountLegalEntityIdDecoded, cancellationToken);

        RequestPermissionsViewModel model = response;

        model.CancelLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!;
        model.BackLink = Url.RouteUrl(RouteNames.EmployerDetails, new { ukprn, accountlegalentityid })!;

        return View(model);
    }

    [HttpPost]
    public IActionResult Index(RequestPermissionsViewModel model)
    {
        return RedirectToAction("Index", new { model.Ukprn, model.AccountLegalEntityId });
    }
}
