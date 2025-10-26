using FluentValidation;

namespace FastVocab.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Request.Id)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Request.FullName)
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.FullName));
    }
}
