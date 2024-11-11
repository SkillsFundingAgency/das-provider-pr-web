using FluentValidation;
using SFA.DAS.Provider.PR.Web.Models;
using static SFA.DAS.Provider.PR.Web.Constants.SetPermissions;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class RequestPermissionsSubmitModelValidator : AbstractValidator<RequestPermissionsSubmitModel>
{
    public const string MatchesExistingPermissionErrorMessage = "These are your current permissions. Select new permissions";
    public const string BothSelectionsAreNoErrorMessage = "You must select yes for at least one permission for add apprentice records or recruit apprentices";

    public RequestPermissionsSubmitModelValidator()
    {
        RuleFor(s => s.PermissionToAddCohorts)
            .Must(ChangePermissionsNotChanged)
            .WithMessage(MatchesExistingPermissionErrorMessage)
            .Must(AddPermissionsBothNoFalse)
            .WithMessage(BothSelectionsAreNoErrorMessage);
    }

    private static bool ChangePermissionsNotChanged(RequestPermissionsSubmitModel viewModel, string? addRecords)
    {
        return 
            !(viewModel.PermissionToAddCohorts == viewModel.ExistingPermissionToAddCohorts && 
            viewModel.PermissionToRecruit == viewModel.ExistingPermissionToAddCohorts);
    }

    private static bool AddPermissionsBothNoFalse(RequestPermissionsSubmitModel viewModel, string? addRecords)
    {
        return !(viewModel.PermissionToAddCohorts == AddRecords.No && viewModel.PermissionToRecruit == RecruitApprentices.No);
    }
}
