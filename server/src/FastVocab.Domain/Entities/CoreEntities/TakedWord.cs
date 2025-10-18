using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities.CoreEntities;

public class TakedWord : EntityBase<int>
{
    public Guid UserId { get; set; }
    public virtual AppUser? User { get; set; }
    public int WordId { get; set; }
    public virtual Word? Word { get; set; }
    public int RepetitionCount { get; set; }

    // Độ dễ của từ (Easiness Factor - mặc định 2.5)
    public double EasinessFactor { get; set; } = 2.5;

    // Ngày ôn cuối
    public DateTime? LastReviewed { get; set; }

    // Ngày ôn tiếp theo
    public DateTime? NextReview { get; set; }

    // Kết quả lần ôn gần nhất
    public ReviewGrade? LastGrade { get; set; }
}
public enum ReviewGrade
{
    Hard = 1,
    Good = 3,
    Easy = 5
}