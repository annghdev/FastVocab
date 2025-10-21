namespace FastVocab.Shared.DTOs.WordLists;

/// <summary>
/// Request DTO for updating a WordList
/// </summary>
public record UpdateWordListRequest
{
    public int Id { get; set; }
    public string Name { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
};

