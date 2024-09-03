using FluentValidation.TestHelper;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Validators;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Validators;
public class SearchByPayeSubmitViewModelValidatorTests
{
    private const string ValidAorn = "1234567890123";
    private const string ValidPaye = "111/111111";

    [TestCase("111/1")]
    [TestCase("111/ABCDEFGHIJ")]
    public void ValidPayeModel_IsValid(string payeRef)
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = payeRef,
            Aorn = ValidAorn
        };

        var sut = new SearchByPayeSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void NoPayeInModel()
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = string.Empty,
            Aorn = ValidAorn
        };

        var sut = new SearchByPayeSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Paye)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.NoPayeErrorMessage);
    }

    [TestCase("1/1")]
    [TestCase("11/1")]
    [TestCase("11A/1")]
    [TestCase("A11/1")]
    [TestCase("1AA/1")]
    [TestCase("11A/1")]
    [TestCase("A1A1")]
    [TestCase("1")]
    [TestCase("12")]
    [TestCase("123/")]
    [TestCase("11A/1234567")]
    [TestCase("1A1/1234567")]
    [TestCase("A11/12345678")]
    [TestCase("AAA/")]
    [TestCase("AAA/1")]
    [TestCase("A1A/1234567")]
    [TestCase("A11/12345678")]
    [TestCase("A11/123456789")]
    [TestCase("A11/1234567890")]
    public void InvalidPayeInModel(string payeRef)
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = payeRef,
            Aorn = ValidAorn
        };

        var sut = new SearchByPayeSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Paye)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.InvalidPayeErrorMessage);

    }

    [TestCase("1")]
    [TestCase("1234567890AB")]
    [TestCase("1234567890ABCD")]
    public void InvalidAornInModel(string aorn)
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = ValidPaye,
            Aorn = aorn
        };

        var sut = new SearchByPayeSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Aorn)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.InvalidAornErrorMessage);
    }

    [Test]
    public void NoAornInModel()
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = "123/1",
            Aorn = string.Empty
        };

        var sut = new SearchByPayeSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Aorn)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.NoAornErrorMessage);
    }

    [Test]
    public void NoPayeOrAornInModel()
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = string.Empty,
            Aorn = string.Empty
        };

        var sut = new SearchByPayeSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Aorn)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.NoAornErrorMessage);

        result.ShouldHaveValidationErrorFor(c => c.Paye)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.NoPayeErrorMessage);
    }


    [Test]
    public void InvalidPayeAndInvalidAornInModel()
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = "1",
            Aorn = "1"
        };

        var sut = new SearchByPayeSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Aorn)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.InvalidAornErrorMessage);

        result.ShouldHaveValidationErrorFor(c => c.Paye)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.InvalidPayeErrorMessage);
    }
}
