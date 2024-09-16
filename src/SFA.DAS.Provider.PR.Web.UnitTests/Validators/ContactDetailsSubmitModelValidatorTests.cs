using FluentValidation.TestHelper;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Validators;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Validators;
public class ContactDetailsSubmitModelValidatorTests
{
    private const string ValidFirstName = "Joe";
    private const string ValidLastName = "Cool";

    [Test]
    public void ContactDetailsModel_IsValid()
    {
        var model = new ContactDetailsSubmitModel()
        {
            FirstName = ValidFirstName,
            LastName = ValidLastName
        };

        var sut = new ContactDetailsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void NoFirstNameInModel()
    {
        var model = new ContactDetailsSubmitModel()
        {
            FirstName = string.Empty,
            LastName = ValidLastName
        };

        var sut = new ContactDetailsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.FirstName)
            .WithErrorMessage(ContactDetailsSubmitModelValidator.FirstNameEmptyErrorMessage);
    }

    [TestCase("a#")]
    [TestCase("a$")]
    [TestCase("a^")]
    [TestCase("a=")]
    [TestCase("a+")]
    [TestCase("a\\")]
    [TestCase("a/")]
    [TestCase("a<")]
    [TestCase("a>")]
    public void FirstNameInvalidInModel(string firstName)
    {
        var model = new ContactDetailsSubmitModel()
        {
            FirstName = firstName,
            LastName = ValidLastName
        };

        var sut = new ContactDetailsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.FirstName)
            .WithErrorMessage(ContactDetailsSubmitModelValidator.FirstNameMustExcludeSpecialCharacters);
    }

    [Test]
    public void NoLastNameInModel()
    {
        var model = new ContactDetailsSubmitModel()
        {
            FirstName = ValidFirstName,
            LastName = string.Empty
        };

        var sut = new ContactDetailsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.LastName)
            .WithErrorMessage(ContactDetailsSubmitModelValidator.LastNameEmptyErrorMessage);
    }

    [TestCase("a#")]
    [TestCase("a$")]
    [TestCase("a^")]
    [TestCase("a=")]
    [TestCase("a+")]
    [TestCase("a\\")]
    [TestCase("a/")]
    [TestCase("a<")]
    [TestCase("a>")]
    public void LastNameInvalidInModel(string lastName)
    {
        var model = new ContactDetailsSubmitModel()
        {
            FirstName = ValidFirstName,
            LastName = lastName
        };

        var sut = new ContactDetailsSubmitModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.LastName)
            .WithErrorMessage(ContactDetailsSubmitModelValidator.LastNameMustExcludeSpecialCharacters);
    }
}
