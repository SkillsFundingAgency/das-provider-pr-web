using FluentValidation.TestHelper;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Validators;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Validators;
public class SearchByEmailSubmitModelValidatorTests
{
    [TestCase("test@account.com", "")]
    [TestCase("test//@account.com", "")]
    [TestCase("", SearchByEmailSubmitModelValidator.NoEmailErrorMessage)]
    [TestCase("test", SearchByEmailSubmitModelValidator.InvalidEmailErrorMessage)]
    [TestCase("test test@account.com", SearchByEmailSubmitModelValidator.InvalidEmailErrorMessage)]
    // [TestCase("aaaa@NonExistentDomain50c2413d-e8e4-4330-9859-222567ad0f64.co.uk", SearchByEmailSubmitModelValidator.InvalidDomainErrorMessage)]
    public void ValidEmailInModel_IsValid(string email, string validationMessage)
    {
        var model = new SearchByEmailSubmitModel()
        {
            Email = email
        };

        var sut = new SearchByEmailSubmitModelValidator();
        var result = sut.TestValidate(model);

        if (validationMessage == string.Empty)
        {
            result.ShouldNotHaveAnyValidationErrors();
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.Email)
                .WithErrorMessage(validationMessage);
        }
    }
}
