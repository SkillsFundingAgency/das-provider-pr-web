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

    [TestCase(SetPermissions.AddRecords.Yes, SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.Yes, SetPermissions.RecruitApprentices.Yes)]
    [TestCase(SetPermissions.AddRecords.No, SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.Yes, SetPermissions.RecruitApprentices.Yes )]
    [TestCase(SetPermissions.AddRecords.No, SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.Yes, SetPermissions.RecruitApprentices.YesWithReview )]
    [TestCase(SetPermissions.AddRecords.Yes, SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.YesWithReview, SetPermissions.RecruitApprentices.Yes)]
    [TestCase(SetPermissions.AddRecords.Yes, SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.No, SetPermissions.RecruitApprentices.Yes)]
    [TestCase(SetPermissions.AddRecords.Yes, SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.YesWithReview, SetPermissions.RecruitApprentices.No)]
    public void Validate_AnyPermissionChanges_IsValid(string existingCohortPErmission, string newCohortPermission, string existingRecruitPermission, string newRecruitPermission)
    {
        var model = new RequestPermissionsSubmitModel()
        {
            PermissionToAddCohorts = newCohortPermission,
            ExistingPermissionToAddCohorts = existingCohortPErmission,
            PermissionToRecruit = newRecruitPermission,
            ExistingPermissionToRecruit = existingRecruitPermission
        };

        var sut = new RequestPermissionsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(a => a.PermissionToAddCohorts);
    }

    public void RequestPermissionsSubmitModelValidator_NoSelections_ReturnsInvalid()
    {
        var model = new RequestPermissionsSubmitModel()
        {
            PermissionToAddCohorts = SetPermissions.AddRecords.No,
            PermissionToRecruit = SetPermissions.RecruitApprentices.No,
            ExistingPermissionToAddCohorts = SetPermissions.AddRecords.No,
            ExistingPermissionToRecruit = SetPermissions.RecruitApprentices.No
        };
        var sut = new RequestPermissionsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(a => a.PermissionToAddCohorts)
            .WithErrorMessage(RequestPermissionsSubmitModelValidator.BothSelectionsAreNoErrorMessage);
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
