using FluentValidation;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class CheckPermissionsSubmitModelValidator : AbstractValidator<ChangePermissionsSubmitModel>
{
    public const string BothSelectionsAreNoErrorMessage = "You must select yes for at least one permission";

    public CheckPermissionsSubmitModelValidator()
    {
        RuleFor(s => s.PermissionToAddCohorts)
            .Cascade(CascadeMode.Stop)
            .Must(AddPermissionsBothNoFalse)
            .WithMessage(BothSelectionsAreNoErrorMessage);
    }

    private static bool AddPermissionsBothNoFalse(ChangePermissionsSubmitModel viewModel, string? addRecords)
    {
        return !(viewModel.PermissionToAddCohorts == SetPermissions.AddRecords.No && viewModel.PermissionToRecruit == SetPermissions.RecruitApprentices.No);
    }
}
