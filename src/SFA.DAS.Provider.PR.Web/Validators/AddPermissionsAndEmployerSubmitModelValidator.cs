using FluentValidation;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class AddPermissionsAndEmployerSubmitModelValidator : AbstractValidator<AddPermissionsAndEmployerSubmitModel>
{
    public const string BothSelectionsAreNoErrorMessage = "You must select yes for at least one permission";

    public AddPermissionsAndEmployerSubmitModelValidator()
    {
        RuleFor(s => s.PermissionToAddCohorts)
            .Cascade(CascadeMode.Stop)
            .Must(HasAtLeastOnePermission)
            .WithMessage(BothSelectionsAreNoErrorMessage);
    }

    private static bool HasAtLeastOnePermission(AddPermissionsAndEmployerSubmitModel model, string? addRecords)
    {
        return !(model.PermissionToAddCohorts == SetPermissions.AddRecords.No && model.PermissionToRecruit == SetPermissions.RecruitApprentices.No);
    }
}
