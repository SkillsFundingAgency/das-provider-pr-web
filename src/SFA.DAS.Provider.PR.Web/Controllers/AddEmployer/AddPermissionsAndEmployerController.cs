using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Domain.OuterApi.Requests.Commands;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Extensions;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;
using SFA.DAS.Provider.PR.Web.Services;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer", Name = RouteNames.AddPermissionsAndEmployer)]
public class AddPermissionsAndEmployerController(IOuterApiClient _outerApiClient, ISessionService _sessionService, IValidator<AddPermissionsAndEmployerSubmitModel> _validator) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/AddPermissionsAndEmployer.cshtml";
    public const string ViewPathSent = "~/Views/AddEmployer/AddEmployerConfirmation.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (sessionModel?.Email == null || sessionModel.AccountLegalEntityId == null)
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var viewModel = GetViewModel(ukprn, sessionModel);

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

        return RedirectToRoute(RouteNames.AddEmployerConfirmation, new { ukprn });
    }

    [HttpGet]
    [Route("/{ukprn}/addEmployer/permissionsRequested", Name = RouteNames.AddEmployerConfirmation)]
    public async Task<IActionResult> AddEmployerAndPermissionsRequested([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (sessionModel?.Email == null || sessionModel.AccountLegalEntityId == null || sessionModel.PermissionToAddCohorts == null)
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var userRef = User.GetUserId();
        var operations = OperationsMappingService.MapDescriptionsToOperations(sessionModel);

        var command = new AddAccountRequestCommand
        {
            AccountId = sessionModel.AccountId!.Value,
            AccountLegalEntityId = sessionModel.AccountLegalEntityId.Value,
            EmployerContactEmail = sessionModel.Email,
            Operations = operations,
            RequestedBy = userRef!,
            Ukprn = ukprn
        };

        await _outerApiClient.AddRequest(command, cancellationToken);

        var viewModel = GetSentViewModel(ukprn, sessionModel);

        _sessionService.Delete<AddEmployerSessionModel>();

        return View(ViewPathSent, viewModel);
    }

    private AddPermissionsAndEmployerViewModel GetViewModel(int ukprn, AddEmployerSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!;

        var permissionToAddCohorts = sessionModel.PermissionToAddCohorts ?? SetPermissions.AddRecords.Yes;

        var permissionToRecruit = sessionModel.PermissionToRecruit ?? SetPermissions.RecruitApprentices.Yes;

        return new AddPermissionsAndEmployerViewModel
        {
            Email = sessionModel.Email,
            LegalName = sessionModel.AccountLegalEntityName!,
            Ukprn = ukprn,
            CancelLink = cancelLink,
            PermissionToAddCohorts = permissionToAddCohorts,
            PermissionToRecruit = permissionToRecruit
        };
    }

    private AddEmployerAndPermissionsSentViewModel GetSentViewModel(int ukprn, AddEmployerSessionModel sessionModel)
    {
        var viewEmployersLink = Url.RouteUrl(RouteNames.Home, new { ukprn })!;
        return new AddEmployerAndPermissionsSentViewModel
        {
            Email = sessionModel.Email,
            LegalName = sessionModel.AccountLegalEntityName!,
            Ukprn = ukprn,
            ViewEmployersLink = viewEmployersLink
        };
    }
}
