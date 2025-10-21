namespace FastVocab.Shared.DTOs.WordLists;

/// <summary>
/// Request DTO for creating a new WordList
/// </summary>
public record CreateWordListRequest
{
    public int CollectionId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
};

