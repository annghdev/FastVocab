using FastVocab.Shared.DTOs.WordLists;

namespace FastVocab.Shared.DTOs.Collections;

/// <summary>
/// DTO for displaying Collection information
/// </summary>
public record CollectionDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? TargetAudience { get; init; }
    public string? DifficultyLevel { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsHiding { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public IEnumerable<WordListDto> WordLists { get; init; } = [];
}

