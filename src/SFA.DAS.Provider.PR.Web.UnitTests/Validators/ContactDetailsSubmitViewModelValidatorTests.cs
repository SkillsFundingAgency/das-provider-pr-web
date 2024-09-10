using FluentValidation.TestHelper;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Validators;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Validators;
public class ContactDetailsSubmitViewModelValidatorTests
{
    private const string ValidFirstName = "Joe";
    private const string ValidLastName = "Cool";

    [Test]
    public void ContactDetailsModel_IsValid()
    {
        var model = new ContactDetailsSubmitViewModel()
        {
            FirstName = ValidFirstName,
            LastName = ValidLastName
        };

        var sut = new ContactDetailsSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void NoFirstNameInModel()
    {
        var model = new ContactDetailsSubmitViewModel()
        {
            FirstName = string.Empty,
            LastName = ValidLastName
        };

        var sut = new ContactDetailsSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.FirstName)
            .WithErrorMessage(ContactDetailsSubmitViewModelValidator.FirstNameEmptyErrorMessage);
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
        var model = new ContactDetailsSubmitViewModel()
        {
            FirstName = firstName,
            LastName = ValidLastName
        };

        var sut = new ContactDetailsSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.FirstName)
            .WithErrorMessage(ContactDetailsSubmitViewModelValidator.FirstNameMustExcludeSpecialCharacters);
    }

    [Test]
    public void NoLastNameInModel()
    {
        var model = new ContactDetailsSubmitViewModel()
        {
            FirstName = ValidFirstName,
            LastName = string.Empty
        };

        var sut = new ContactDetailsSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.LastName)
            .WithErrorMessage(ContactDetailsSubmitViewModelValidator.LastNameEmptyErrorMessage);
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
        var model = new ContactDetailsSubmitViewModel()
        {
            FirstName = ValidFirstName,
            LastName = lastName
        };

        var sut = new ContactDetailsSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.LastName)
            .WithErrorMessage(ContactDetailsSubmitViewModelValidator.LastNameMustExcludeSpecialCharacters);
    }
}
