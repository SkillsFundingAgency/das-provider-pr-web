using FluentValidation;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class AddPermissionsAndEmployerSubmitViewModelValidator : AbstractValidator<AddPermissionsAndEmployerSubmitViewModel>
{
    public const string BothSelectionsAreNoErrorMessage = "You must select yes for at least one permission";

    public AddPermissionsAndEmployerSubmitViewModelValidator()
    {
        RuleFor(s => s.PermissionToAddCohorts)
            .Cascade(CascadeMode.Stop)
            .Must(AddPermissionsBothNoFalse)
            .WithMessage(BothSelectionsAreNoErrorMessage);
    }

    private static bool AddPermissionsBothNoFalse(AddPermissionsAndEmployerSubmitViewModel viewModel, string? addRecords)
    {
        return !(viewModel.PermissionToAddCohorts == SetPermissions.AddRecords.No && viewModel.PermissionToRecruit == SetPermissions.RecruitApprentices.No);
    }
}
