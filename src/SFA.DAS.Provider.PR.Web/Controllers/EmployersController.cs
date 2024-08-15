using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("[controller]")]
public class EmployersController(IOuterApiClient _outerApiclient, IOptions<ApplicationSettings> _applicationSettingsOption) : Controller
{
    public const string NoRelationshipsHomePage = "NoEmployerRelationships";

    [Route("{ukprn:int}", Name = RouteNames.Employers)]
    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromQuery] EmployersSubmitModel submitModel, CancellationToken cancellationToken)
    {
        GetProviderRelationshipsResponse response = await _outerApiclient.GetProviderRelationships(ukprn, submitModel.ToQueryString(), cancellationToken);

        if (response.HasAnyRelationships)
        {
            return View(new EmployersViewModel(response, Url, ukprn, submitModel.PageNumber.GetValueOrDefault(), _applicationSettingsOption.Value.EmployersPageSize));
        }

        return View(NoRelationshipsHomePage, new NoRelationshipsHomeViewModel(Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!));
    }
}
