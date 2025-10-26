using FastVocab.Domain.Constants;
using FluentValidation;

namespace FastVocab.Application.Features.Words.Queries.GetWordsByLevel;

/// <summary>
/// Validator for GetWordsByLevelQuery
/// </summary>
public class GetWordsByLevelValidator : AbstractValidator<GetWordsByLevelQuery>
{
    private static readonly string[] ValidWordLevels = 
    {
        WordLevels.A1, WordLevels.A2, WordLevels.B1,
        WordLevels.B2, WordLevels.C1, WordLevels.C2
    };

    public GetWordsByLevelValidator()
    {
        RuleFor(x => x.Level)
            .NotEmpty().WithMessage("Level is required.")
            .Must(level => ValidWordLevels.Contains(level))
            .WithMessage($"Level must be one of: {string.Join(", ", ValidWordLevels)}");
    }
}
