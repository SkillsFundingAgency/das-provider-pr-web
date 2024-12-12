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

[Route("/{ukprn}/addEmployer/contactDetails", Name = RouteNames.AddEmployerContactDetails)]
public class ContactDetailsController(ISessionService _sessionService, IValidator<ContactDetailsSubmitModel> _validator) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/ContactDetails.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var viewModel = GetViewModel(ukprn, sessionModel);


        if (!string.IsNullOrEmpty(sessionModel.FirstName))
        {
            viewModel.FirstName = sessionModel.FirstName;
        }

        if (!string.IsNullOrEmpty(sessionModel.LastName))
        {
            viewModel.LastName = sessionModel.LastName;
        }

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public IActionResult Index([FromRoute] int ukprn, ContactDetailsSubmitModel submitModel, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var result = _validator.Validate(submitModel);

        if (!result.IsValid)
        {
            var viewModel = GetViewModel(ukprn, sessionModel);
            result.AddToModelState(ModelState);
            return View(ViewPath, viewModel);
        }

        sessionModel.FirstName = submitModel.FirstName!.Trim();
        sessionModel.LastName = submitModel.LastName!.Trim();
        _sessionService.Set(sessionModel);

        return RedirectToRoute(RouteNames.CheckEmployerDetails, new { ukprn });
    }


    private ContactDetailsViewModel GetViewModel(int ukprn, AddEmployerSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var backLink = Url.RouteUrl(RouteNames.AddEmployerSearchByPaye, new { ukprn });
        if (sessionModel.IsCheckDetailsVisited)
        {
            backLink = Url.RouteUrl(RouteNames.CheckEmployerDetails, new { ukprn });
        }

        return new ContactDetailsViewModel { CancelLink = cancelLink!, BackLink = backLink!, Ukprn = ukprn, Aorn = sessionModel.Aorn!, Paye = sessionModel.Paye!, OrganisationName = sessionModel.OrganisationName! };
    }
}
