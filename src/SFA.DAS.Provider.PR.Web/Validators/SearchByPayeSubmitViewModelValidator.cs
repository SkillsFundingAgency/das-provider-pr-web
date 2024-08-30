using FluentValidation;
using SFA.DAS.Provider.PR.Application.Constants;
using SFA.DAS.Provider.PR.Web.Models.AddEmployer;

namespace SFA.DAS.Provider.PR.Web.Validators;

public class SearchByPayeSubmitViewModelValidator : AbstractValidator<SearchByPayeSubmitViewModel>
{
    public const string NoPayeErrorMessage = "Enter an employer PAYE reference";
    public const string InvalidPayeErrorMessage = "Enter an employer PAYE reference in the correct format";
    public const string NoAornErrorMessage = "Enter an Accounts office reference";
    public const string InvalidAornErrorMessage = "Enter your Accounts Office reference in the correct format";

    public SearchByPayeSubmitViewModelValidator()
    {
        RuleFor(s => s.Paye)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(NoPayeErrorMessage)
            .Matches(RegularExpressions.PayeRegex)
            .WithMessage(InvalidPayeErrorMessage);

        RuleFor(s => s.Aorn)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(NoAornErrorMessage)
            .Matches(RegularExpressions.AornRegex)
            .WithMessage(InvalidAornErrorMessage);
    }
}