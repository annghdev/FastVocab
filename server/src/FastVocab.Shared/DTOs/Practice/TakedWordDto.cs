namespace FastVocab.Shared.DTOs.Practice;

/// <summary>
/// DTO for displaying TakedWord (user's word progress) information
/// </summary>
public record TakedWordDto
{
    public int Id { get; init; }
    public Guid UserId { get; init; }
    public int WordId { get; init; }
    public string WordText { get; init; } = string.Empty;
    public int RepetitionCount { get; init; }
    public double EasinessFactor { get; init; }
    public DateTimeOffset? LastReviewed { get; init; }
    public DateTimeOffset? NextReview { get; init; }
    public int? LastGrade { get; init; }
    public bool IsDueForReview { get; init; }
}

