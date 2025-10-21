using FluentValidation;
using FastVocab.Application.Features.Collections.Commands.UpdateWordList;

public class UpdateWordListValidator : AbstractValidator<UpdateWordListCommand>
{
    public UpdateWordListValidator()
    {
        RuleFor(x => x.Request.Id)
            .GreaterThan(0);

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(x => x.Request.ImageUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Request.ImageUrl));
    }
}
