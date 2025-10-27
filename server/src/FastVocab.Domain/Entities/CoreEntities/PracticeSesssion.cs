using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities.CoreEntities;

public class PracticeSesssion : AuditableEntityBase<int>
{
    public Guid UserId { get; set; }
    public virtual AppUser? User { get; set; }
    public int ListId { get; set; }
    public virtual WordList? List { get; set; }

    public int RepetitionCount { get; set; }

    // Ngày ôn cuối
    public DateTime? LastReviewed { get; set; }

    // Ngày ôn tiếp theo
    public DateTime? NextReview { get; set; }

    public void Submit()
    {
        LastReviewed = DateTime.UtcNow;
        NextReview =  LastReviewed?.AddDays(SRS.FixedSpaced[RepetitionCount]);
        RepetitionCount++;
    }
}
