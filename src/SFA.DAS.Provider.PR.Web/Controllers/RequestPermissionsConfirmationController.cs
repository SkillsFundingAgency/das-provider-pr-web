using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Application.Constants;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("{ukprn:long}/employers/{accountlegalentityid}/requestpermissions/confirmation", Name = RouteNames.RequestPermissionsConfirmation)]
public class RequestPermissionsConfirmationController : Controller
{
    [HttpGet]
    public IActionResult Index([FromRoute] long ukprn, [FromRoute]string accountLegalEntityId, CancellationToken cancellationToken)
    {
        var viewModel = new RequestPermissionsConfirmationViewModel
        {
            Ukprn = ukprn,
            AccountLegalEntityName = GetAccountLegalEntityName(),
            EmployersLink = Url.RouteUrl(RouteNames.Employers, new { ukprn, HasPendingRequest = true })!
        };

        return View(viewModel);
    }

    private string GetAccountLegalEntityName()
    {
        return TempData.TryGetValue(TempDataKeys.AccountLegalEntityName, out var value) && value is not null
        ? value.ToString() ?? string.Empty
        : string.Empty;
    }
}
