namespace FastVocab.Shared.DTOs.Topics;

/// <summary>
/// Request DTO for updating an existing Topic
/// </summary>
public record UpdateTopicRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string VnText { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public bool IsHiding { get; init; }
}

