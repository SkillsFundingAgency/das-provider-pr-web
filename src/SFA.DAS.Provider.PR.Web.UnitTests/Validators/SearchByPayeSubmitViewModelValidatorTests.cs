using FluentValidation.TestHelper;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;
using SFA.DAS.Provider.PR.Web.Validators;

namespace SFA.DAS.Provider.PR_Web.UnitTests.Validators;
public class SearchByPayeSubmitViewModelValidatorTests
{
    [TestCase("111/1", "1234567890123")]
    [TestCase("111/12", "1234567890123")]
    [TestCase("111/ABC", "1234567890123")]
    [TestCase("111/ABCD", "1234567890123")]
    [TestCase("111/ABCDE", "1234567890123")]
    [TestCase("111/ABCDEF", "1234567890123")]
    [TestCase("111/ABCDEFG", "1234567890123")]
    [TestCase("111/ABCDEFGH", "1234567890123")]
    [TestCase("111/ABCDEFGHI", "1234567890123")]
    [TestCase("111/ABCDEFGHIJ", "1234567890123")]
    public void ValidDetailsInModel_IsValid(string payeRef, string aorn)
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = payeRef,
            Aorn = aorn
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
            Aorn = "1234567890123"
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
            Aorn = "1234567890123"
        };

        var sut = new SearchByPayeSubmitViewModelValidator();
        var result = sut.TestValidate(model);

        result.ShouldHaveValidationErrorFor(c => c.Paye)
            .WithErrorMessage(SearchByPayeSubmitViewModelValidator.InvalidPayeErrorMessage);

    }

    [TestCase("1")]
    [TestCase("12")]
    [TestCase("123")]
    [TestCase("1234")]
    [TestCase("12345")]
    [TestCase("123456")]
    [TestCase("1234567")]
    [TestCase("12345678")]
    [TestCase("123456789")]
    [TestCase("1234567890")]
    [TestCase("1234567890A")]
    [TestCase("1234567890AB")]
    [TestCase("1234567890ABCD")]
    public void InvalidAornInModel(string aorn)
    {
        var model = new SearchByPayeSubmitViewModel()
        {
            Paye = "123/1234567",
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
