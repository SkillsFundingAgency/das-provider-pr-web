using FluentValidation;
using SFA.DAS.Provider.PR.Application.Constants;
using SFA.DAS.Provider.PR.Application.Services;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class SearchByEmailSubmitViewModelValidator : AbstractValidator<SearchByEmailSubmitViewModel>
{
    public const string InvalidDomainErrorMessage = "Enter an email address with a valid domain";
    public const string NoEmailErrorMessage = "Enter an email address";
    public const string InvalidEmailErrorMessage = "Enter an email address in the correct format, like name@example.com";

    public SearchByEmailSubmitViewModelValidator()
    {
        RuleFor(s => s.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(NoEmailErrorMessage)
            .Matches(RegularExpressions.EmailRegex)
            .WithMessage(InvalidEmailErrorMessage)
            .Must(IsDomainValid)
            .WithMessage(InvalidDomainErrorMessage);
    }

    private bool IsDomainValid(string email)
    {
        return EmailCheckingService.IsValidDomain(email);
    }
}
