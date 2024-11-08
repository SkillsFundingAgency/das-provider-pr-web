using FluentValidation.TestHelper;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models;
using SFA.DAS.Provider.PR.Web.Validators;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Validators;

public sealed class RequestPermissionsSubmitModelValidatorTests
{
    [Test]
    public void RequestPermissionsSubmitModelValidator_MatchingPermissions_ReturnInvalid()
    {
        var model = new RequestPermissionsSubmitModel()
        {
            PermissionToAddCohorts = SetPermissions.AddRecords.Yes,
            PermissionToRecruit = SetPermissions.RecruitApprentices.Yes,
            ExistingPermissionToAddCohorts = SetPermissions.AddRecords.Yes,
            ExistingPermissionToRecruit = SetPermissions.RecruitApprentices.Yes
        };
        var sut = new RequestPermissionsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(a => a.PermissionToAddCohorts)
            .WithErrorMessage(RequestPermissionsSubmitModelValidator.MatchesExistingPermissionErrorMessage);
    }

    [Test]
    public void RequestPermissionsSubmitModelValidator_EmptySelections_ReturnInvalid()
    {
        var model = new RequestPermissionsSubmitModel()
        {
            PermissionToAddCohorts = string.Empty,
            PermissionToRecruit = string.Empty,
            ExistingPermissionToAddCohorts = SetPermissions.AddRecords.Yes,
            ExistingPermissionToRecruit = SetPermissions.RecruitApprentices.Yes
        };
        var sut = new RequestPermissionsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(a => a.PermissionToRecruit)
            .WithErrorMessage(RequestPermissionsSubmitModelValidator.RequestPermissionsRecruitApprenticesErrorMessage);

        result.ShouldHaveValidationErrorFor(a => a.PermissionToAddCohorts)
            .WithErrorMessage(RequestPermissionsSubmitModelValidator.RequestPermissionsAddRecordsErrorMessage);
    }

    [Test]
    public void RequestPermissionsSubmitModelValidator_Valid()
    {
        var model = new RequestPermissionsSubmitModel()
        {
            PermissionToAddCohorts = SetPermissions.AddRecords.Yes,
            PermissionToRecruit = SetPermissions.RecruitApprentices.Yes,
            ExistingPermissionToAddCohorts = SetPermissions.AddRecords.No,
            ExistingPermissionToRecruit = SetPermissions.RecruitApprentices.No
        };
        var sut = new RequestPermissionsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(a => a.PermissionToRecruit);
        result.ShouldNotHaveValidationErrorFor(a => a.PermissionToAddCohorts);
    }
}
