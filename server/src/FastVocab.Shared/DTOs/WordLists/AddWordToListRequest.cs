namespace FastVocab.Shared.DTOs.WordLists;

/// <summary>
/// Request DTO for adding a word to a WordList
/// </summary>
public record AddWordToListRequest
{
    public int WordListId { get; init; }
    public int WordId { get; init; }
};
