using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Responses;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer/searchByEmail", Name = RouteNames.AddEmployerSearchByEmail)]
public class SearchByEmailController(IOuterApiClient _outerApiClient, ISessionService _sessionService, IValidator<SearchByEmailSubmitViewModel> _validator) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/SearchByEmail.cshtml";
    public const string MultipleAccountsShutterPathViewPath = "~/Views/AddEmployer/MultipleAccountsShutterPage.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {

        var viewModel = GetViewModel(ukprn);
        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromRoute] int ukprn, SearchByEmailSubmitViewModel submitViewModel, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(submitViewModel);
        var viewModel = GetViewModel(ukprn);
        viewModel.Email = submitViewModel.Email;

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(ViewPath, viewModel);
        }

        var relationshipByEmail = await _outerApiClient.GetRelationshipByEmail(submitViewModel.Email!, ukprn, cancellationToken);

        var hasMultipleAccounts = HasMultipleAccounts(relationshipByEmail);

        if (hasMultipleAccounts != null && hasMultipleAccounts.Value)
        {
            return RedirectToRoute(RouteNames.AddEmployerMultipleAccounts, new { ukprn });
        }

        _sessionService.Set(new AddEmployerSessionModel(submitViewModel.Email!));

        return View(ViewPath, viewModel);
    }

    [HttpGet]
    [Route("multipleAccounts", Name = RouteNames.AddEmployerMultipleAccounts)]
    public IActionResult MultipleAccountsShutterPage([FromRoute] int ukprn)
    {
        var shutterViewModel = new MultipleAccountsShutterPageViewModel(
            Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!
            );

        return View(MultipleAccountsShutterPathViewPath, shutterViewModel);
    }

    private SearchByEmailViewModel GetViewModel(int ukprn)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var backLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        return new SearchByEmailViewModel(cancelLink!, backLink!, ukprn);
    }

    private static bool? HasMultipleAccounts(GetRelationshipByEmailResponse response)
    {

        if (!response.HasUserAccount)
        {
            return null;
        }

        if (response.HasOneEmployerAccount != null
            && response.HasOneEmployerAccount.Value
            && response.HasOneLegalEntity != null
            && response.HasOneLegalEntity.Value)
        {
            return false;
        }

        return true;
    }
}