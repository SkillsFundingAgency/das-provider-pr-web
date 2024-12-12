using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using SFA.DAS.Encoding;
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
public class SearchByEmailController(IOuterApiClient _outerApiClient, ISessionService _sessionService, IEncodingService encodingService, IValidator<SearchByEmailSubmitModel> _validator) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/SearchByEmail.cshtml";
    public const string MultipleAccountsShutterPathViewPath = "~/Views/AddEmployer/ShutterPages/MultipleAccountsShutterPage.cshtml";
    public const string OneAccountNoRelationshipViewPath = "~/Views/AddEmployer/OneAccountNoRelationship.cshtml";
    public const string EmailLinkedToAccountWithRelationshipShutterPageViewPath = "~/Views/AddEmployer/ShutterPages/EmailLinkedToAccountWithRelationship.cshtml";
    public const string EmailSearchInviteAlreadySentShutterPageViewPath = "~/Views/AddEmployer/ShutterPages/EmailSearchInviteAlreadySentShutterPage.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var viewModel = GetViewModel(ukprn);
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (sessionModel != null && sessionModel.IsCheckDetailsVisited)
        {
            return RedirectToRoute(RouteNames.CheckEmployerDetails, new { ukprn });
        }

        if (!string.IsNullOrEmpty(sessionModel?.Email))
        {
            viewModel.Email = sessionModel.Email;
            sessionModel = new AddEmployerSessionModel { Email = sessionModel.Email! };
            _sessionService.Set(sessionModel);
        }

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromRoute] int ukprn, SearchByEmailSubmitModel submitModel, CancellationToken cancellationToken)
    {
        submitModel.Email = submitModel.Email?.Trim();

        var result = await _validator.ValidateAsync(submitModel);

        if (!result.IsValid)
        {
            var viewModel = GetViewModel(ukprn);
            viewModel.Email = submitModel.Email;
            result.AddToModelState(ModelState);
            return View(ViewPath, viewModel);
        }

        var sessionModel = new AddEmployerSessionModel { Email = submitModel.Email! };
        _sessionService.Set(sessionModel);

        var relationshipByEmail = await _outerApiClient.GetRelationshipByEmail(submitModel.Email!, ukprn, cancellationToken);

        if (relationshipByEmail.HasActiveRequest)
        {
            return RedirectToRoute(RouteNames.EmailSearchInviteAlreadySent, new { ukprn });
        }

        if (!relationshipByEmail.HasUserAccount!.Value)
        {
            return RedirectToRoute(RouteNames.AddEmployerSearchByPaye, new { ukprn });
        }

        var hasMultipleAccounts = HasMultipleAccounts(relationshipByEmail);

        if (hasMultipleAccounts)
        {
            return RedirectToRoute(RouteNames.AddEmployerMultipleAccounts, new { ukprn });
        }

        sessionModel.AccountLegalEntityId = relationshipByEmail.AccountLegalEntityId;
        sessionModel.AccountLegalEntityName = relationshipByEmail.AccountLegalEntityName;
        sessionModel.AccountId = relationshipByEmail.AccountId;

        _sessionService.Set(sessionModel);

        return RedirectToRoute(relationshipByEmail.HasRelationship is true
            ? RouteNames.EmailLinkedToAccountWithRelationship :
            RouteNames.OneAccountNoRelationship, new { ukprn });
    }

    [HttpGet]
    [Route("emailSearchInviteAlreadySent", Name = RouteNames.EmailSearchInviteAlreadySent)]
    public async Task<IActionResult> EmailSearchInviteAlreadySent([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();
        var email = sessionModel?.Email;

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var requestResponse = await _outerApiClient.GetRequestByUkprnAndEmail(ukprn, email, cancellationToken);
        var response = requestResponse.GetContent();

        long? accountLegalEntityId = response?.AccountLegalEntityId;

        string? employerName = response?.EmployerOrganisationName?.ToUpper();

        if (string.IsNullOrEmpty(employerName))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var employerAccountLink = string.Empty;
        if (accountLegalEntityId != null && response.RequestType == RequestType.Permission.ToString())
        {
            var accountLegalEntityIdEncoded = encodingService.Encode(accountLegalEntityId.Value, EncodingType.PublicAccountLegalEntityId);
            employerAccountLink = Url.RouteUrl(RouteNames.EmployerDetails, new { ukprn, accountLegalEntityId = accountLegalEntityIdEncoded })!;
        }
        else
        {
            employerAccountLink = Url.RouteUrl(RouteNames.EmployerDetailsByRequestId, new { ukprn, response!.RequestId });
        }

        var shutterViewModel = new EmailSearchInviteAlreadySentShutterPageViewModel(email, employerName, employerAccountLink!);

        return View(EmailSearchInviteAlreadySentShutterPageViewPath, shutterViewModel);
    }

    [HttpGet]
    [Route("emailLinkedToAccountWithRelationship", Name = RouteNames.EmailLinkedToAccountWithRelationship)]
    public IActionResult EmailLinkedToAccountWithRelationshipShutterPage([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();
        var email = sessionModel?.Email;
        var employerName = sessionModel?.AccountLegalEntityName!;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(employerName) || sessionModel!.AccountLegalEntityId == null)
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var accountLegalEntityIdEncoded = encodingService.Encode(sessionModel.AccountLegalEntityId.Value, EncodingType.PublicAccountLegalEntityId);
        var employerAccountLink = Url.RouteUrl(RouteNames.EmployerDetails, new { ukprn, accountLegalEntityId = accountLegalEntityIdEncoded })!;
        var shutterViewModel = new EmailLinkedToAccountWithRelationshipShutterPageViewModel(email, employerName, employerAccountLink);

        return View(EmailLinkedToAccountWithRelationshipShutterPageViewPath, shutterViewModel);
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
    [OutputCache(Duration = 0, NoStore = true)]
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
            ContinueLink = Url.RouteUrl(RouteNames.AddPermissionsAndEmployer, new { ukprn })!,
            Ukprn = ukprn,
            Email = sessionModel.Email
        };

        return View(OneAccountNoRelationshipViewPath, viewModel);
    }

    private SearchByEmailModel GetViewModel(int ukprn)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var backLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        return new SearchByEmailModel { CancelLink = cancelLink!, BackLink = backLink!, Ukprn = ukprn };
    }

    private static bool HasMultipleAccounts(GetRelationshipByEmailResponse response)
    {
        return !(response.HasOneEmployerAccount.GetValueOrDefault()
                && response.HasOneLegalEntity.GetValueOrDefault());
    }
}