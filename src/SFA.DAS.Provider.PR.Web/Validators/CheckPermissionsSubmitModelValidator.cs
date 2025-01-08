using FluentValidation;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class CheckPermissionsSubmitModelValidator : AbstractValidator<AddPermissionsSubmitModel>
{
    public const string BothSelectionsAreNoErrorMessage = "You must select yes for at least one permission";

    public CheckPermissionsSubmitModelValidator()
    {
        RuleFor(s => s.PermissionToAddCohorts)
            .Cascade(CascadeMode.Stop)
            .Must(HasAtLeastOnePermission)
            .WithMessage(BothSelectionsAreNoErrorMessage);

        RuleFor(s => s.PermissionToRecruit)
            .Cascade(CascadeMode.Stop)
            .Must(HasAtLeastOnePermission)
            .WithMessage(BothSelectionsAreNoErrorMessage);
    }

    private static bool HasAtLeastOnePermission(AddPermissionsSubmitModel viewModel, string? addRecords)
    {
        return !(viewModel.PermissionToAddCohorts == SetPermissions.AddRecords.No && viewModel.PermissionToRecruit == SetPermissions.RecruitApprentices.No);
    }
}
