namespace FastVocab.Shared.DTOs.Topics;

/// <summary>
/// DTO for displaying Topic information
/// </summary>
public record TopicDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string VnText { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public bool IsHiding { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

