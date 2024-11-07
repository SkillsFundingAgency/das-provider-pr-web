using FluentValidation;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class RequestPermissionsSubmitModelValidator : AbstractValidator<RequestPermissionsSubmitModel>
{
    public const string RequestPermissionsAddRecordsErrorMessage = "Select the permissions you want to set for Add apprentice records";
    public const string RequestPermissionsRecruitApprenticesErrorMessage = "Select the permissions you want to set for Recruit apprentices";
    public const string BothSelectionsAreNoErrorMessage = "You must select yes for at least one permission for add apprentice records or recruit apprentices";

    public RequestPermissionsSubmitModelValidator()
    {
        RuleFor(s => s.PermissionToAddCohorts)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(RequestPermissionsAddRecordsErrorMessage)
            .Must(AddPermissionsBothNoFalse)
            .WithMessage(BothSelectionsAreNoErrorMessage);

        RuleFor(s => s.PermissionToRecruit)
            .NotEmpty()
            .WithMessage(RequestPermissionsRecruitApprenticesErrorMessage);
    }

    private static bool AddPermissionsBothNoFalse(RequestPermissionsSubmitModel viewModel, string? addRecords)
    {
        return !(viewModel.PermissionToAddCohorts == SetPermissions.RecruitApprentices.No && viewModel.PermissionToRecruit == SetPermissions.RecruitApprentices.No);
    }
}
