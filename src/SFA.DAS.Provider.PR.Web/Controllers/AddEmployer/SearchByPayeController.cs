using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer/searchByPaye", Name = RouteNames.AddEmployerSearchByPaye)]
public class SearchByPayeController(IOuterApiClient _outerApiClient, ISessionService _sessionService, IValidator<SearchByPayeSubmitViewModel> _validator) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/SearchByPaye.cshtml";


    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var viewModel = GetViewModel(ukprn);

        viewModel.Email = sessionModel!.Email;

        if (!string.IsNullOrEmpty(sessionModel.Paye))
        {
            viewModel.Paye = sessionModel.Paye;
        }

        if (!string.IsNullOrEmpty(sessionModel.Aorn))
        {
            viewModel.Aorn = sessionModel.Aorn;
        }

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromRoute] int ukprn, SearchByPayeSubmitViewModel submitViewModel, CancellationToken cancellationToken)
    {

        var result = _validator.Validate(submitViewModel);

        if (!result.IsValid)
        {
            var viewModel = GetViewModel(ukprn);
            viewModel.Email = submitViewModel.Email;
            viewModel.Paye = submitViewModel.Paye;
            viewModel.Aorn = submitViewModel.Aorn;
            result.AddToModelState(ModelState);
            return View(ViewPath, viewModel);
        }
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        sessionModel.Paye = submitViewModel.Paye;
        sessionModel.Aorn = submitViewModel.Aorn;
        _sessionService.Set(sessionModel);

        var encodedPaye = Uri.EscapeDataString(sessionModel.Paye!);

        var relationshipsRequest = await _outerApiClient.GetProviderRelationshipsByUkprnPayeAorn(ukprn, sessionModel.Aorn!, encodedPaye, cancellationToken);

        bool? hasOneLegalEntity = relationshipsRequest?.HasOneLegalEntity;

        if (hasOneLegalEntity is false)
        {
            return RedirectToRoute(RouteNames.AddEmployerMultipleAccounts, new { ukprn });
        }

        return RedirectToRoute(RouteNames.AddEmployerSearchByPaye, new { ukprn });
    }


    private SearchByPayeViewModel GetViewModel(int ukprn)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var backLink = Url.RouteUrl(RouteNames.AddEmployerSearchByEmail, new { ukprn });
        return new SearchByPayeViewModel { CancelLink = cancelLink!, BackLink = backLink!, Ukprn = ukprn };
    }

}
