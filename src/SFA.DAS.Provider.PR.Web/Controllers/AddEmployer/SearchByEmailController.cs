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
    public const string OneAccountNoRelationshipViewPath = "~/Views/AddEmployer/OneAccountNoRelationship.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var viewModel = GetViewModel(ukprn);
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();
        if (!string.IsNullOrEmpty(sessionModel?.Email))
        {
            viewModel.Email = sessionModel.Email;
        }

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromRoute] int ukprn, SearchByEmailSubmitViewModel submitViewModel, CancellationToken cancellationToken)
    {
        var result = _validator.Validate(submitViewModel);


        if (!result.IsValid)
        {
            var viewModel = GetViewModel(ukprn);
            viewModel.Email = submitViewModel.Email;
            result.AddToModelState(ModelState);
            return View(ViewPath, viewModel);
        }

        _sessionService.Set(new AddEmployerSessionModel(submitViewModel.Email!));

        var relationshipByEmail = await _outerApiClient.GetRelationshipByEmail(submitViewModel.Email!, ukprn, cancellationToken);

        if (!relationshipByEmail.HasUserAccount)
        {
            return RedirectToRoute(RouteNames.AddEmployerSearchByEmail, new { ukprn });
        }

        var hasMultipleAccounts = HasMultipleAccounts(relationshipByEmail);

        if (hasMultipleAccounts)
        {
            return RedirectToRoute(RouteNames.AddEmployerMultipleAccounts, new { ukprn });
        }

        if (relationshipByEmail.HasRelationship != null && !relationshipByEmail.HasRelationship.Value)
        {
            return RedirectToRoute(RouteNames.OneAccountNoRelationship, new { ukprn });
        }

        return RedirectToRoute(RouteNames.AddEmployerSearchByEmail, new { ukprn });
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

    [HttpGet]
    [Route("accountFound", Name = RouteNames.OneAccountNoRelationship)]
    public IActionResult OneAccountNoRelationshipFound([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();
        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var viewModel = new OneAccountNoRelationshipViewModel
        {
            CancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!,
            BackLink = Url.RouteUrl(RouteNames.AddEmployerSearchByEmail, new { ukprn })!,
            ContinueLink = Url.RouteUrl(RouteNames.OneAccountNoRelationship, new { ukprn })!,
            Ukprn = ukprn,
            Email = sessionModel.Email
        };

        return View(OneAccountNoRelationshipViewPath, viewModel);
    }

    private SearchByEmailViewModel GetViewModel(int ukprn)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var backLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        return new SearchByEmailViewModel { CancelLink = cancelLink!, BackLink = backLink!, Ukprn = ukprn };
    }

    private static bool HasMultipleAccounts(GetRelationshipByEmailResponse response)
    {
        return !(response.HasOneEmployerAccount.GetValueOrDefault()
                && response.HasOneLegalEntity.GetValueOrDefault());
    }
}