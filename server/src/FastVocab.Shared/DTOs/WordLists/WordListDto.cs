using FastVocab.Shared.DTOs.Words;

namespace FastVocab.Shared.DTOs.WordLists;

/// <summary>
/// DTO for displaying WordList information
/// </summary>
public record WordListDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int CollectionId { get; init; }
    public int WordCount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public IEnumerable<WordDto> Words { get; set; } = [];
};

