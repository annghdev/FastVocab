namespace FastVocab.Shared.DTOs.WordLists;

/// <summary>
/// Request DTO for creating a new WordList
/// </summary>
public record CreateWordListRequest
{
    public string Name { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public int CollectionId { get; init; }
    public List<int>? WordIds { get; init; } // Words to add to this list
}

