namespace FastVocab.Shared.DTOs.Collections;

/// <summary>
/// Request DTO for updating an existing Collection
/// </summary>
public record UpdateCollectionRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? TargetAudience { get; init; }
    public string? DifficultyLevel { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsHiding { get; init; }
}

