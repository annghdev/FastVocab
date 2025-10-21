using FluentValidation;
using FastVocab.Application.Features.Collections.Commands.CreateCollection;

namespace FastVocab.Application.Features.Topics.Commands.CreateTopic;

/// <summary>
/// Validator for CreateCollectionCommand
/// </summary>
public class CreateCollectionValidator : AbstractValidator<CreateCollectionCommand>
{
    public CreateCollectionValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(3, 200).WithMessage("Name must be between 3 and 200 characters.");

        RuleFor(x => x.Request.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Request.TargetAudience)
            .MaximumLength(100).WithMessage("TargetAudience cannot exceed 100 characters.");

        RuleFor(x => x.Request.DifficultyLevel)
            .MaximumLength(10).WithMessage("DifficultyLevel cannot exceed 10 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.DifficultyLevel));

        RuleFor(x => x.Request.ImageUrl)
            .MaximumLength(500).WithMessage("ImageUrl cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.ImageUrl));
    }
}
