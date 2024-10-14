using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Encoding;
using SFA.DAS.Provider.PR.Domain.Interfaces;
using SFA.DAS.Provider.PR.Web.Authorization;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Models.Session;

namespace SFA.DAS.Provider.PR.Web.Controllers.AddEmployer;

[Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]

[Route("/{ukprn}/addEmployer/searchByPaye", Name = RouteNames.AddEmployerSearchByPaye)]
public class SearchByPayeController(IOuterApiClient _outerApiClient, ISessionService _sessionService, IEncodingService encodingService, IValidator<SearchByPayeSubmitModel> _validator) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/SearchByPaye.cshtml";
    public const string PayeAornShutterPathViewPath = "~/Views/AddEmployer/ShutterPages/PayeAornShutterPage.cshtml";
    public const string InviteAlreadySentShutterPathViewPath = "~/Views/AddEmployer/ShutterPages/InviteAlreadySentShutterPage.cshtml";
    public const string PayeAornMatchedEmailNotLinkedShutterPathViewPath = "~/Views/AddEmployer/ShutterPages/PayeAornMatchedEmailNotLinkedShutterPage.cshtml";
    public const string PayeAornMatchedEmailNotLinkedHasRelationshipShutterPathViewPath = "~/Views/AddEmployer/ShutterPages/PayeAornMatchedEmailNotLinkedHasRelationshipShutterPage.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        if (sessionModel.IsCheckDetailsVisited)
        {
            return RedirectToRoute(RouteNames.CheckEmployerDetails, new { ukprn });
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
    public async Task<IActionResult> Index([FromRoute] int ukprn, SearchByPayeSubmitModel submitModel, CancellationToken cancellationToken)
    {

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            var viewModel = GetViewModel(ukprn);
            viewModel.Email = submitModel.Email;
            viewModel.Paye = submitModel.Paye;
            viewModel.Aorn = submitModel.Aorn;
            result.AddToModelState(ModelState);
            return View(ViewPath, viewModel);
        }
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        sessionModel.Paye = submitModel.Paye;
        sessionModel.Aorn = submitModel.Aorn;
        sessionModel.IsCheckDetailsVisited = false;
        _sessionService.Set(sessionModel);

        var encodedPaye = Uri.EscapeDataString(sessionModel.Paye!);

        var relationshipsRequest = await _outerApiClient.GetProviderRelationshipsByUkprnPayeAorn(ukprn, sessionModel.Aorn!, encodedPaye, cancellationToken);

        bool? hasActiveRequest = relationshipsRequest.HasActiveRequest;

        if (hasActiveRequest is true)
        {
            var requestResponse = await _outerApiClient.GetRequest(ukprn, sessionModel.Paye!, cancellationToken);

            var request = requestResponse.GetContent();
            if (request.AccountLegalEntityId != null)
            {
                sessionModel.AccountLegalEntityId = request.AccountLegalEntityId!.Value;
                _sessionService.Set(sessionModel);
            }

            return RedirectToRoute(RouteNames.AddEmployerInvitationAlreadySent, new { ukprn });
        }

        bool? hasInvalidPaye = relationshipsRequest.HasInvalidPaye;

        if (hasInvalidPaye is true)
        {
            return RedirectToRoute(RouteNames.AddEmployerPayeAornNotCorrect, new { ukprn });
        }

        var accountDoesNotExist = relationshipsRequest.Account == null;

        if (accountDoesNotExist)
        {
            sessionModel.OrganisationName = relationshipsRequest.Organisation?.Name;
            _sessionService.Set(sessionModel);
            return RedirectToRoute(RouteNames.AddEmployerContactDetails, new { ukprn });
        }

        /// If you get here, there is definitely one or more legal entities, as 'accountDoesNotExist' is false
        bool hasOneLegalEntity = relationshipsRequest.HasOneLegalEntity!.Value;

        if (hasOneLegalEntity is false)
        {
            return RedirectToRoute(RouteNames.AddEmployerMultipleAccounts, new { ukprn });
        }

        sessionModel.AccountLegalEntityId = relationshipsRequest.AccountLegalEntityId;
        sessionModel.AccountLegalEntityName = relationshipsRequest.AccountLegalEntityName;
        sessionModel.AccountId = relationshipsRequest.Account!.AccountId;
        sessionModel.OrganisationName = relationshipsRequest.Organisation?.Name;
        _sessionService.Set(sessionModel);

        if (relationshipsRequest.HasRelationship!.Value)
        {
            return RedirectToRoute(RouteNames.PayeAornMatchedEmailNotLinkedHasRelationshipLink, new { ukprn });
        }

        return RedirectToRoute(RouteNames.PayeAornMatchedEmailNotLinkedNoRelationshipLink, new { ukprn });
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

        var requestResponse = await _outerApiClient.GetRequest(ukprn, sessionModel.Paye, cancellationToken);

        var request = requestResponse.GetContent();

        if (!requestResponse.ResponseMessage.IsSuccessStatusCode || request.RequestType != createAccount)
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        string employerName = request.EmployerOrganisationName!;

        var backLink = Url.RouteUrl(RouteNames.AddEmployerSearchByPaye, new { ukprn })!;

        long? accountLegalEntityId = sessionModel.AccountLegalEntityId;

        string employerAccountLink;
        if (accountLegalEntityId != null)
        {
            var accountLegalEntityIdEncoded = encodingService.Encode(accountLegalEntityId.Value, EncodingType.PublicAccountLegalEntityId);
            employerAccountLink = Url.RouteUrl(RouteNames.EmployerDetails, new { ukprn, accountLegalEntityId = accountLegalEntityIdEncoded })!;
        }
        else
        {
            employerAccountLink = Url.RouteUrl(RouteNames.EmployerDetailsByRequestId, new { request.RequestId })!;
        }

        var shutterViewModel = new InviteAlreadySentShutterPageViewModel(
            employerName,
            sessionModel.Paye,
            sessionModel.Aorn!,
            sessionModel.Email,
            backLink,
            employerAccountLink
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

    [HttpGet]
    [Route("payeAornMatchedEmailNotLinked", Name = RouteNames.PayeAornMatchedEmailNotLinkedNoRelationshipLink)]
    public IActionResult PayeAornMatchedEmailNotLinkedShutterPage([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();
        if (string.IsNullOrEmpty(sessionModel?.Paye))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn })!;
        var continueLink = Url.RouteUrl(RouteNames.AddPermissionsAndEmployer, new { ukprn })!;
        var shutterViewModel = new PayeAornMatchedEmailNotLinkedViewModel
        {
            EmployerName = sessionModel.OrganisationName!,
            PayeReference = sessionModel.Paye!,
            Aorn = sessionModel.Aorn!,
            Email = sessionModel.Email,
            CancelLink = cancelLink,
            ContinueLink = continueLink
        };

        return View(PayeAornMatchedEmailNotLinkedShutterPathViewPath, shutterViewModel);
    }

    [HttpGet]
    [Route("payeAornMatchedEmailNotLinkedRelationship", Name = RouteNames.PayeAornMatchedEmailNotLinkedHasRelationshipLink)]
    public IActionResult PayeAornMatchedEmailNotLinkedHasRelationshipShutterPage([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();
        if (string.IsNullOrEmpty(sessionModel?.Paye))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var accountLegalEntityId = sessionModel.AccountLegalEntityId!.Value;

        var accountLegalEntityIdEncoded = encodingService.Encode(accountLegalEntityId, EncodingType.PublicAccountLegalEntityId);
        var employerAccountLink = Url.RouteUrl(RouteNames.EmployerDetails, new { ukprn, accountLegalEntityId = accountLegalEntityIdEncoded })!;


        var shutterViewModel = new PayeAornMatchedEmailNotLinkedHasRelationshipViewModel
        {
            EmployerName = sessionModel.OrganisationName!.ToUpper(),
            PayeReference = sessionModel.Paye!,
            Aorn = sessionModel.Aorn!,
            Email = sessionModel.Email,
            EmployerAccountLink = employerAccountLink
        };

        return View(PayeAornMatchedEmailNotLinkedHasRelationshipShutterPathViewPath, shutterViewModel);
    }

    private SearchByPayeModel GetViewModel(int ukprn)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var backLink = Url.RouteUrl(RouteNames.AddEmployerSearchByEmail, new { ukprn });
        return new SearchByPayeModel { CancelLink = cancelLink!, BackLink = backLink!, Ukprn = ukprn };
    }

}
