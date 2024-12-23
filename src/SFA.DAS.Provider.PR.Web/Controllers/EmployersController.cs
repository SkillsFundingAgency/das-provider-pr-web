﻿using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.PR.Application.Constants;
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
    public async Task<IActionResult> Index([FromRoute] long ukprn, [FromQuery] EmployersSubmitModel submitModel, CancellationToken cancellationToken)
    {
        if (TempData.ContainsKey(TempDataKeys.PermissionsRequestId))
        {
            TempData.Remove(TempDataKeys.PermissionsRequestId);
        }

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
                Employers = BuildEmployers(response.Employers.ToList(), ukprn),
                TotalCount = "employer".ToQuantity(response.TotalCount)
            };

            return base.View(model);
        }

        return View(NoRelationshipsHomePage, new NoRelationshipsHomeViewModel(Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!));
    }

    private List<EmployerPermissionViewModel> BuildEmployers(List<ProviderRelationshipModel> employers, long ukprn)
    {
        var employerList = new List<EmployerPermissionViewModel>();
        foreach (var employer in employers)
        {
            var newEmployer = (EmployerPermissionViewModel)employer;
            if (newEmployer.HasPendingRequest)
            {
                newEmployer.EmployerDetailsUrl = Url.RouteUrl(RouteNames.EmployerDetailsByRequestId,
                    new { ukprn, employer.RequestId })!;
            }
            else
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
