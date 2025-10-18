namespace FastVocab.Shared.DTOs.Words;

/// <summary>
/// Request DTO for creating a new Word
/// </summary>
public record CreateWordRequest
{
    public string Text { get; init; } = string.Empty;
    public string Meaning { get; init; } = string.Empty;
    public string? Definition { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
    public string? Example1 { get; init; }
    public string? Example2 { get; init; }
    public string? Example3 { get; init; }
    public string? ImageUrl { get; init; }
    public string? AudioUrl { get; init; }
    public List<int>? TopicIds { get; init; } // Topics to associate with this word
}

