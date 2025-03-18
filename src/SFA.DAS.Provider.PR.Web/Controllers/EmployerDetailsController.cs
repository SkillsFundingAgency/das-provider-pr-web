using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;
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

        if (IsActivePermissionsRequest(response))
        {
            return RedirectToRoute(RouteNames.EmployerDetails,
                new { ukprn, response.GetContent().AccountLegalEntityId });
        }

        EmployerDetailsViewModel model = response.GetContent();

        if (model.AccountLegalEntityId.HasValue)
        {
            model.AccountLegalEntityPublicHashedId = encodingService.Encode(model.AccountLegalEntityId.Value, EncodingType.PublicAccountLegalEntityId);
        }

        model.EmployersLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!;

        return View(model);
    }

    private static bool IsActivePermissionsRequest(Response<GetRequestsByRequestIdResponse> response)
    {
        var responseContent = response.GetContent();

        return string.Equals(responseContent.RequestType, RequestType.Permission.ToString(), StringComparison.CurrentCultureIgnoreCase) &&
               (string.Equals(responseContent.Status, RequestStatus.New.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(responseContent.Status, RequestStatus.Sent.ToString(), StringComparison.CurrentCultureIgnoreCase));
    }
}
