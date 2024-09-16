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

        if (string.IsNullOrEmpty(sessionModel?.Email))
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

        if (string.IsNullOrEmpty(sessionModel?.Email))
        {
            return RedirectToRoute(RouteNames.AddEmployerStart, new { ukprn });
        }

        return RedirectToRoute(RouteNames.CheckEmployerDetails, new { ukprn });
    }

    private CheckDetailsViewModel GetViewModel(int ukprn, AddEmployerSessionModel sessionModel)
    {
        var cancelLink = Url.RouteUrl(RouteNames.AddEmployerStart, new { ukprn });
        var changeEmployerNameLink = string.Empty;
        var changePermissionsLink = string.Empty;

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

        var permissionToAddCohortsText = SetCohortPermissionText(sessionModel.PermissionToAddCohorts);
        var permissionToRecruitText = SetRecruitPermissionText(sessionModel.PermissionToRecruit);

        return new CheckDetailsViewModel(ukprn, sessionModel.OrganisationName!, sessionModel.Paye!, sessionModel.Aorn!,
            sessionModel.Email, sessionModel.FirstName!, sessionModel.LastName!,
            sessionModel.PermissionToAddCohorts, permissionToAddCohortsText,
            sessionModel.PermissionToRecruit, permissionToRecruitText,
            changeEmployerNameLink, changePermissionsLink, cancelLink!);
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
}
