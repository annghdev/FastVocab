using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities.CoreEntities;

public class Collection : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? TargetAudience { get; set; }
    public string? DifficultyLevel { get; set; }
    public string? ImageUrl { get; set; }

    public bool IsHiding { get; set; }
    public virtual ICollection<WordList>? WordLists { get; set; }
}
