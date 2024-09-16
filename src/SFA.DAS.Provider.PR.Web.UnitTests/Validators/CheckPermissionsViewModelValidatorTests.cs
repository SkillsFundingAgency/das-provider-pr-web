using FluentValidation.TestHelper;
using SFA.DAS.Provider.PR.Web.Constants;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Validators;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Validators;
public class CheckPermissionsSubmitModelValidatorTests
{
    [TestCase(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.Yes)]
    [TestCase(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.Yes)]
    [TestCase(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.YesWithReview)]
    [TestCase(SetPermissions.AddRecords.No, SetPermissions.RecruitApprentices.YesWithReview)]
    [TestCase(SetPermissions.AddRecords.Yes, SetPermissions.RecruitApprentices.No)]
    public void AddRecordsAndRecruitApprenticesSet_Valid(string addRecordsSelection, string recruitApprenticesSelection)
    {
        var model = new ChangePermissionsSubmitModel
        {
            PermissionToAddCohorts = addRecordsSelection,
            PermissionToRecruit = recruitApprenticesSelection
        };

        var sut = new CheckPermissionsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void AddRecordsNo_RecruitApprenticesNo_InvalidWithExpectedMessage()
    {
        var model = new ChangePermissionsSubmitModel
        {
            PermissionToAddCohorts = SetPermissions.AddRecords.No,
            PermissionToRecruit = SetPermissions.RecruitApprentices.No
        };

        var sut = new CheckPermissionsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.PermissionToAddCohorts)
            .WithErrorMessage(CheckPermissionsSubmitModelValidator.BothSelectionsAreNoErrorMessage);
    }
}