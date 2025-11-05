namespace FastVocab.Shared.DTOs.Words;

/// <summary>
/// Request DTO for creating a new Word
/// </summary>
public class CreateWordRequest
{
    public string Text { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string? Definition { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string? Example1 { get; set; }
    public string? Example2 { get; set; }
    public string? Example3 { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public List<int>? TopicIds { get; set; } // Topics to associate with this word
}

