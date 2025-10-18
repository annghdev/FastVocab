namespace FastVocab.Shared.DTOs.Topics;

/// <summary>
/// Request DTO for creating a new Topic
/// </summary>
public record CreateTopicRequest
{
    public string Name { get; init; } = string.Empty;
    public string VnText { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
}

