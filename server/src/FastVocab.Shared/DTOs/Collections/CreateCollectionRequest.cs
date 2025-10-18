namespace FastVocab.Shared.DTOs.Collections;

/// <summary>
/// Request DTO for creating a new Collection
/// </summary>
public record CreateCollectionRequest
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? TargetAudience { get; init; }
    public string? DifficultyLevel { get; init; }
    public string? ImageUrl { get; init; }
}

