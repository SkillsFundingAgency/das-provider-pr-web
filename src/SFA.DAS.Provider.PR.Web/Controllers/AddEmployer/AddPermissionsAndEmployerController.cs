using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer/addPermissions", Name = RouteNames.AddPermissionsAndEmployer)]
public class AddPermissionsAndEmployerController(ISessionService _sessionService, IValidator<AddPermissionsAndEmployerSubmitViewModel> _validator) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/AddPermissionsAndEmployer.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (sessionModel?.Email == null || sessionModel.AccountLegalEntityId == null)
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var viewModel = GetViewModel(ukprn, sessionModel);

        viewModel.PermissionToAddCohorts = sessionModel.PermissionToAddCohorts != null
            ? sessionModel.PermissionToAddCohorts
            : SetPermissions.AddRecords.Yes;

        viewModel.PermissionToRecruit = sessionModel.PermissionToRecruit != null
            ? sessionModel.PermissionToRecruit
            : SetPermissions.RecruitApprentices.Yes;

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public IActionResult Index([FromRoute] int ukprn, AddPermissionsAndEmployerViewModel submitViewModel)
    {
        var result = _validator.Validate(submitViewModel);

        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();
        if (sessionModel?.Email == null || sessionModel.AccountLegalEntityId == null)
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

        return RedirectToRoute(RouteNames.AddPermissionsAndEmployer, new { ukprn });
    }


    private AddPermissionsAndEmployerViewModel GetViewModel(int ukprn, AddEmployerSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!;

        return new AddPermissionsAndEmployerViewModel { Email = sessionModel.Email, LegalName = sessionModel.AccountLegalEntityName!, Ukprn = ukprn, CancelLink = cancelLink };
    }
}
