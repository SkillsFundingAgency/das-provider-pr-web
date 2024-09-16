using FluentValidation;
using SFA.DAS.Provider.PR.Application.Constants;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class ContactDetailsSubmitModelValidator : AbstractValidator<ContactDetailsSubmitModel>
{
    public const string FirstNameEmptyErrorMessage = "You must enter a first name";
    public const string FirstNameMustExcludeSpecialCharacters = "First name must include valid characters";
    public const string LastNameEmptyErrorMessage = "You must enter a last name";
    public const string LastNameMustExcludeSpecialCharacters = "Last name must include valid characters";

    public ContactDetailsSubmitModelValidator()
    {
        RuleFor(s => s.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(FirstNameEmptyErrorMessage)
            .Matches(RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(FirstNameMustExcludeSpecialCharacters);

        RuleFor(s => s.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(LastNameEmptyErrorMessage)
            .Matches(RegularExpressions.ExcludedCharactersRegex)
            .WithMessage(LastNameMustExcludeSpecialCharacters);

    }
}