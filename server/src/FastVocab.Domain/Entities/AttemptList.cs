using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities;

public class AttemptList : AuditableEntityBase<int>
{
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public int ListId { get; set; }
    public WordList? List { get; set; }

    public int RepetitionCount { get; set; }

    // Ngày ôn cuối
    public DateTime? LastReviewed { get; set; }

    // Ngày ôn tiếp theo
    public DateTime? NextReview { get; set; }

    public void SetNextTime()
    {
        LastReviewed = NextReview;
        NextReview?.AddDays(SRS.FixedSpaced[RepetitionCount]);
    }
}
