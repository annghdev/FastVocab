namespace FastVocab.Shared.DTOs.Practice;

/// <summary>
/// Request DTO for reviewing a word in practice session
/// </summary>
public record ReviewWordRequest
{
    public Guid UserId { get; init; }
    public int WordId { get; init; }
    public int Grade { get; init; } // 0-5 based on SM-2 algorithm
}

