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

[Route("/{ukprn}/addEmployer/checkDetails", Name = RouteNames.CheckEmployerDetails)]
public class CheckDetailsController(ISessionService _sessionService) : Controller
{
    public const string ViewPath = "~/Views/AddEmployer/CheckDetails.cshtml";

    [HttpGet]
    public IActionResult Index([FromRoute] int ukprn)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (!IsCompleteFlow(sessionModel))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        var viewModel = GetViewModel(ukprn, sessionModel);

        return View(ViewPath, viewModel);
    }

    [HttpPost]
    public IActionResult Index([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        var sessionModel = _sessionService.Get<AddEmployerSessionModel>();

        if (!IsCompleteFlow(sessionModel))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        return RedirectToRoute(RouteNames.CheckEmployerDetails, new { ukprn });
    }

    private CheckDetailsViewModel GetViewModel(int ukprn, AddEmployerSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var changeEmployerNameLink = Url.RouteUrl(RouteNames.AddEmployerContactDetails, new { ukprn });
        var changePermissionsLink = Url.RouteUrl(RouteNames.ChangePermissions, new { ukprn });

        if (sessionModel.PermissionToAddCohorts == null)
        {
            sessionModel.PermissionToAddCohorts = SetPermissions.AddRecords.Yes;
            _sessionService.Set(sessionModel);
        }

        if (sessionModel.PermissionToRecruit == null)
        {
            sessionModel.PermissionToRecruit = SetPermissions.RecruitApprentices.Yes;
            _sessionService.Set(sessionModel);
        }

        if (!sessionModel.IsCheckDetailsVisited)
        {
            sessionModel.IsCheckDetailsVisited = true;
            _sessionService.Set(sessionModel);
        }

        var permissionToAddCohortsText = SetCohortPermissionText(sessionModel.PermissionToAddCohorts);
        var permissionToRecruitText = SetRecruitPermissionText(sessionModel.PermissionToRecruit);

        return new CheckDetailsViewModel
        {
            Ukprn = ukprn,
            OrganisationName = sessionModel.OrganisationName?.ToUpper()!,
            Paye = sessionModel.Paye!,
            Aorn = sessionModel.Aorn!,
            Email = sessionModel.Email,
            FirstName = sessionModel.FirstName!,
            LastName = sessionModel.LastName!,
            PermissionToAddCohorts = sessionModel.PermissionToAddCohorts,
            PermissionToAddCohortsText = permissionToAddCohortsText,
            PermissionToRecruit = sessionModel.PermissionToRecruit,
            PermissionToRecruitText = permissionToRecruitText,
            ChangeEmployerNameLink = changeEmployerNameLink!,
            ChangePermissionsLink = changePermissionsLink!,
            CancelLink = cancelLink!
        };
    }

    private static string SetCohortPermissionText(string sessionModelPermissionToAddCohorts)
    {
        return sessionModelPermissionToAddCohorts == SetPermissions.AddRecords.Yes
            ? SetPermissionsText.CohortsPermissionText
            : SetPermissionsText.NoPermissionText;
    }
    private static string SetRecruitPermissionText(string sessionModelPermissionToRecruit)
    {
        return sessionModelPermissionToRecruit switch
        {
            SetPermissions.RecruitApprentices.Yes => SetPermissionsText.RecruitmentPermissionText,
            SetPermissions.RecruitApprentices.YesWithReview => SetPermissionsText.RecruitmentWithReviewPermissionText,
            _ => SetPermissionsText.NoPermissionText
        };
    }

    private static bool IsCompleteFlow(AddEmployerSessionModel? sessionModel)
    {
        if (string.IsNullOrEmpty(sessionModel?.Email)) return false;

        if (string.IsNullOrEmpty(sessionModel.Paye) || string.IsNullOrEmpty(sessionModel.Aorn)) return false;

        if (string.IsNullOrWhiteSpace(sessionModel.OrganisationName)) return false;

        if (string.IsNullOrWhiteSpace(sessionModel.FirstName) || string.IsNullOrEmpty(sessionModel.LastName)) return false;

        return true;
    }
}
