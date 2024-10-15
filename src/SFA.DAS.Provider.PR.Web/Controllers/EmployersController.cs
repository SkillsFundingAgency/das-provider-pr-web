using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestEase;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Extensions;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasProviderAccount))]
[Route("")]
public class EmployersController(IOuterApiClient _outerApiclient, IOptions<ApplicationSettings> _applicationSettingsOption) : Controller
{
    public const string NoRelationshipsHomePage = "NoEmployerRelationships";

    [Route("{ukprn:int}/employers", Name = RouteNames.Employers)]
    [HttpGet]
    public async Task<IActionResult> Index([FromRoute] int ukprn, [FromQuery] EmployersSubmitModel submitModel, CancellationToken cancellationToken)
    {
        var queryParams = submitModel.ToQueryString();
        var pageSize = _applicationSettingsOption.Value.EmployersPageSize;
        queryParams.Add("PageSize", pageSize.ToString());
        GetProviderRelationshipsResponse response = await _outerApiclient.GetProviderRelationships(ukprn, queryParams, cancellationToken);

        if (response.HasAnyRelationships)
        {
            queryParams[nameof(ukprn)] = ukprn.ToString();
            EmployersViewModel model = new()
            {
                Pagination = new(response.TotalCount, pageSize, Url, RouteNames.Employers, submitModel.ConvertToDictionary()),
                ClearFiltersLink = Url.RouteUrl(RouteNames.Employers, new { ukprn })!,
                AddEmployerLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!,
                Employers = await BuildEmployers(response.Employers.ToList(), ukprn, cancellationToken),
                TotalCount = "employer".ToQuantity(response.TotalCount)
            };

            return base.View(model);
        }

        return View(NoRelationshipsHomePage, new NoRelationshipsHomeViewModel(Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!));
    }

    private async Task<List<EmployerPermissionViewModel>> BuildEmployers(List<ProviderRelationshipModel> employers, int ukprn, CancellationToken cancellationToken)
    {
        var employerList = new List<EmployerPermissionViewModel>();
        foreach (var employer in employers)
        {
            var newEmployer = (EmployerPermissionViewModel)employer;
            if (newEmployer.HasPendingRequest)
            {
                Response<GetRequestsByRequestIdResponse> response = await
                    _outerApiclient.GetRequestByRequestId((Guid)employer.RequestId!, cancellationToken);

                if (string.Equals(response.GetContent().RequestType, RequestType.AddAccount.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(response.GetContent().RequestType, RequestType.CreateAccount.ToString(), StringComparison.CurrentCultureIgnoreCase))
                {
                    newEmployer.EmployerDetailsUrl = Url.RouteUrl(RouteNames.EmployerDetailsByRequestId,
                        new { ukprn, response.GetContent().RequestId })!;
                }
            }

            if (newEmployer.EmployerDetailsUrl == null)
            {
                newEmployer.EmployerDetailsUrl =
                    Url.RouteUrl(RouteNames.EmployerDetails,
                        new { ukprn, accountLegalEntityId = employer.AgreementId })!;
            }

            employerList.Add(newEmployer);
        }
        return employerList;
    }
}
