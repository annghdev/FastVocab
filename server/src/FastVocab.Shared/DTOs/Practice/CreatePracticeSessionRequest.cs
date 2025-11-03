namespace FastVocab.Shared.DTOs.Practice;

public record CreatePracticeSessionRequest
{
    public Guid UserId { get; init; }
    public int ListId { get; init; }
}
