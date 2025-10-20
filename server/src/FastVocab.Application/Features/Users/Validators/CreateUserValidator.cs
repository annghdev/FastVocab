using FastVocab.Application.Features.Users.Commands.CreateUser;
using FluentValidation;

namespace FastVocab.Application.Features.Users.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Request.FullName)
            .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.FullName));
    }
}
