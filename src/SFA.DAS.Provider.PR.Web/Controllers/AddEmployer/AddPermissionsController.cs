using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer/changePermissions", Name = RouteNames.ChangePermissions)]
public class AddPermissionsController(ISessionService _sessionService, IValidator<AddPermissionsSubmitModel> _validator) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/AddPermissions.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (sessionModel?.Email == null)
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var viewModel = GetViewModel(ukprn, sessionModel);

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public IActionResult Index([FromRoute] int ukprn, AddPermissionsSubmitModel submitViewModel)
    {
        var result = _validator.Validate(submitViewModel);

        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();
        if (sessionModel?.Email == null)
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        if (!result.IsValid)
        {
            var viewModel = GetViewModel(ukprn, sessionModel);
            result.AddToModelState(ModelState);
            return View(ViewPath, viewModel);
        }

        sessionModel.PermissionToRecruit = submitViewModel.PermissionToRecruit;
        sessionModel.PermissionToAddCohorts = submitViewModel.PermissionToAddCohorts;

        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.CheckEmployerDetails, new { ukprn });
    }


    private AddPermissionsViewModel GetViewModel(int ukprn, AddEmployerSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!;
        var backLink = Url.RouteUrl(RouteNames.CheckEmployerDetails, new { ukprn })!;
        return new AddPermissionsViewModel
        {
            Email = sessionModel.Email,
            OrganisationName = sessionModel.OrganisationName?.ToUpper()!,
            Ukprn = ukprn,
            CancelLink = cancelLink,
            BackLink = backLink,
            PermissionToAddCohorts = sessionModel.PermissionToAddCohorts,
            PermissionToRecruit = sessionModel.PermissionToRecruit
        };
    }
}