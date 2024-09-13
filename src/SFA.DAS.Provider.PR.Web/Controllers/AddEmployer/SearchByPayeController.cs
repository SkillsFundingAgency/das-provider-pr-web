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
    public const string PayeAornShutterPathViewPath = "~/Views/AddEmployer/ShutterPages/PayeAornShutterPage.cshtml";
    public const string InviteAlreadySentShutterPathViewPath = "~/Views/AddEmployer/ShutterPages/InviteAlreadySentShutterPage.cshtml";

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

        bool? hasActiveRequest = relationshipsRequest?.HasActiveRequest;

        if (hasActiveRequest is true)
        {
            return RedirectToRoute(RouteNames.AddEmployerInvitationAlreadySent, new { ukprn });
        }

        bool? hasInvalidPaye = relationshipsRequest?.HasInvalidPaye;

        if (hasInvalidPaye is true)
        {
            return RedirectToRoute(RouteNames.AddEmployerPayeAornNotCorrect, new { ukprn });
        }

        bool? hasOneLegalEntity = relationshipsRequest?.HasOneLegalEntity;

        if (hasOneLegalEntity is false)
        {
            return RedirectToRoute(RouteNames.AddEmployerMultipleAccounts, new { ukprn });
        }

        var hasExistingAccount = relationshipsRequest?.Account != null;

        if (!hasExistingAccount)
        {
            sessionModel.OrganisationName = relationshipsRequest?.Organisation?.Name;
            _sessionService.Set(sessionModel);
            return RedirectToRoute(RouteNames.AddEmployerContactDetails, new { ukprn });
        }

        return RedirectToRoute(RouteNames.AddEmployerSearchByPaye, new { ukprn });
    }


    [HttpGet]
    [Route("invitationSent", Name = RouteNames.AddEmployerInvitationAlreadySent)]
    public async Task<IActionResult> InvitationSentShutterPage([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        const string createAccount = "CreateAccount";
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (string.IsNullOrEmpty(sessionModel?.Paye))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        string employerName;

        var requestResponse = await _outerApiClient.GetRequest(ukprn, sessionModel.Paye, cancellationToken);

        if (requestResponse.ResponseMessage.IsSuccessStatusCode && requestResponse.GetContent().RequestType == createAccount)
        {
            employerName = requestResponse.GetContent().EmployerOrganisationName!;
        }
        else
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var backLink = Url.RouteUrl(RouteNames.AddEmployerSearchByPaye, new { ukprn })!;

        var checkEmployerLink = "";
        var shutterViewModel = new InviteAlreadySentShutterPageViewModel(
            employerName,
            sessionModel.Paye,
            sessionModel.Aorn!,
            sessionModel.Email,
            backLink, checkEmployerLink
        );

        return View(InviteAlreadySentShutterPathViewPath, shutterViewModel);
    }

    [HttpGet]
    [Route("payeAornNotMatched", Name = RouteNames.AddEmployerPayeAornNotCorrect)]
    public IActionResult PayeAornShutterPage([FromRoute] int ukprn)
    {
        var backLink = Url.RouteUrl(RouteNames.AddEmployerSearchByPaye, new { ukprn })!;
        var checkEmployerLink = Url.RouteUrl(RouteNames.AddEmployerSearchByPaye, new { ukprn })!;
        var shutterViewModel = new PayeAornNotCorrectShutterPageViewModel(
            backLink, checkEmployerLink
        );

        return View(PayeAornShutterPathViewPath, shutterViewModel);
    }

    private SearchByPayeViewModel GetViewModel(int ukprn)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var backLink = Url.RouteUrl(RouteNames.AddEmployerSearchByEmail, new { ukprn });
        return new SearchByPayeViewModel { CancelLink = cancelLink!, BackLink = backLink!, Ukprn = ukprn };
    }

}
