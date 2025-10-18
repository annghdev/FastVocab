namespace FastVocab.Shared.DTOs.WordLists;

/// <summary>
/// Request DTO for updating an existing WordList
/// </summary>
public record UpdateWordListRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
    public List<int>? WordIds { get; init; } // Updated word list
}

