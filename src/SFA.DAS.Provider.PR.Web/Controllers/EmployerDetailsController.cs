using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("")]
public class EmployerDetailsController(IOuterApiClient _outerApiclient, IEncodingService encodingService) : Controller
{
    [Route("{ukprn:int}/employers/{accountlegalentityid}", Name = RouteNames.EmployerDetails)]
    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromRoute] string accountlegalentityid,
        CancellationToken cancellationToken)
    {
        var accountLegalEntityIdDecoded = encodingService.Decode(accountlegalentityid, EncodingType.PublicAccountLegalEntityId);

        var requestResponse =
            await _outerApiclient.GetRequestByUkprnAndAccountLegalEntityId(ukprn, accountLegalEntityIdDecoded,
                cancellationToken);
        var request = requestResponse?.GetContent();

        if (request?.RequestId != null &&
            request.RequestId != Guid.Empty &&
            request.RequestType == RequestType.AddAccount &&
           (request.Status == RequestStatus.New || request.Status == RequestStatus.Sent))
        {
            return RedirectToRoute(RouteNames.EmployerDetailsByRequestId, new { ukprn, request.RequestId });
        }

        GetProviderRelationshipResponse response =
            await _outerApiclient.GetProviderRelationship(ukprn, accountLegalEntityIdDecoded, cancellationToken);

        EmployerDetailsViewModel model = response;

        model.EmployersLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!;

        return View(model);
    }

    [Route("{ukprn:int}/employers/{requestid:Guid}", Name = RouteNames.EmployerDetailsByRequestId)]
    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromRoute] Guid requestid,
        CancellationToken cancellationToken)
    {
        var response = await _outerApiclient.GetRequestByRequestId(requestid, cancellationToken);

        if (response.ResponseMessage.StatusCode == HttpStatusCode.NotFound)
        {
            return RedirectToAction("HttpStatusCodeHandler", "Error", new { statusCode = 404 });
        }

        EmployerDetailsViewModel model = response.GetContent();

        model.AccountLegalEntityPublicHashedId =
            encodingService.Encode(model.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId);

        model.EmployersLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!;

        return View(model);
    }
}
