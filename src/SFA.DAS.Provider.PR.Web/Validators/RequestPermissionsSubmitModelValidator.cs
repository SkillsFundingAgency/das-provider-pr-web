using FluentValidation;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class RequestPermissionsSubmitModelValidator : AbstractValidator<RequestPermissionsSubmitModel>
{
    public const string RequestPermissionsAddRecordsErrorMessage = "Select the permissions you want to set for Add apprentice records";
    public const string RequestPermissionsRecruitApprenticesErrorMessage = "Select the permissions you want to set for Recruit apprentices";
    public const string MatchesExistingPermissionErrorMessage = "These are your current permissions. Select new permissions";

    public RequestPermissionsSubmitModelValidator()
    {
        RuleFor(s => s.PermissionToAddCohorts)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(RequestPermissionsAddRecordsErrorMessage);

        RuleFor(s => s.PermissionToRecruit)
            .NotEmpty()
            .WithMessage(RequestPermissionsRecruitApprenticesErrorMessage);

        RuleFor(s => s.PermissionToAddCohorts)
            .Must(ChangePermissionsNotChanged)
            .WithMessage(MatchesExistingPermissionErrorMessage);
    }

    private static bool ChangePermissionsNotChanged(RequestPermissionsSubmitModel viewModel, string? addRecords)
    {
        return 
            !(viewModel.PermissionToAddCohorts == viewModel.ExistingPermissionToAddCohorts && 
            viewModel.PermissionToRecruit == viewModel.ExistingPermissionToAddCohorts);
    }
}
