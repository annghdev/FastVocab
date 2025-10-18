namespace FastVocab.Shared.DTOs.Practice;

/// <summary>
/// DTO for displaying PracticeSession information
/// </summary>
public record PracticeSessionDto
{
    public int Id { get; init; }
    public Guid UserId { get; init; }
    public int ListId { get; init; }
    public string ListName { get; init; } = string.Empty;
    public int RepetitionCount { get; init; }
    public DateTimeOffset? LastReviewed { get; init; }
    public DateTimeOffset? NextReview { get; init; }
    public bool IsDueForReview { get; init; }
}

