using FastVocab.Domain.Constants;
using FluentValidation;

namespace FastVocab.Application.Features.Words.Commands.UpdateWord;

/// <summary>
/// Validator for UpdateWordCommand
/// </summary>
public class UpdateWordValidator : AbstractValidator<UpdateWordCommand>
{
    private static readonly string[] ValidWordTypes = 
    {
        WordTypes.Noun, WordTypes.Pronoun, WordTypes.Verb, 
        WordTypes.Adjective, WordTypes.Adverb, WordTypes.Preposition,
        WordTypes.Conjunction, WordTypes.Article
    };

    private static readonly string[] ValidWordLevels = 
    {
        WordLevels.A1, WordLevels.A2, WordLevels.B1,
        WordLevels.B2, WordLevels.C1, WordLevels.C2
    };

    public UpdateWordValidator()
    {
        RuleFor(x => x.Request.Id)
            .GreaterThan(0).WithMessage("Word ID must be greater than 0.");

        RuleFor(x => x.Request.Text)
            .NotEmpty().WithMessage("Word text is required.")
            .MinimumLength(1).WithMessage("Word text must be at least 1 character.")
            .MaximumLength(100).WithMessage("Word text must not exceed 100 characters.");

        RuleFor(x => x.Request.Meaning)
            .NotEmpty().WithMessage("Vietnamese meaning is required.")
            .MinimumLength(1).WithMessage("Meaning must be at least 1 character.")
            .MaximumLength(500).WithMessage("Meaning must not exceed 500 characters.");

        RuleFor(x => x.Request.Definition)
            .MaximumLength(1000).WithMessage("Definition must not exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.Definition));

        RuleFor(x => x.Request.Type)
            .NotEmpty().WithMessage("Word type is required.")
            .Must(type => ValidWordTypes.Contains(type))
            .WithMessage($"Word type must be one of: {string.Join(", ", ValidWordTypes)}");

        RuleFor(x => x.Request.Level)
            .NotEmpty().WithMessage("Word level is required.")
            .Must(level => ValidWordLevels.Contains(level))
            .WithMessage($"Word level must be one of: {string.Join(", ", ValidWordLevels)}");

        RuleFor(x => x.Request.Example1)
            .MaximumLength(200).WithMessage("Example 1 must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.Example1));

        RuleFor(x => x.Request.Example2)
            .MaximumLength(200).WithMessage("Example 2 must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.Example2));

        RuleFor(x => x.Request.Example3)
            .MaximumLength(200).WithMessage("Example 3 must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Request.Example3));

        RuleFor(x => x.Request.ImageUrl)
            .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Request.ImageUrl))
            .WithMessage("Image URL must be a valid URL.");

        RuleFor(x => x.Request.AudioUrl)
            .MaximumLength(500).WithMessage("Audio URL must not exceed 500 characters.")
            .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.Request.AudioUrl))
            .WithMessage("Audio URL must be a valid URL.");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}

