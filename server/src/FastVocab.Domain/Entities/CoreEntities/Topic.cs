using FastVocab.Domain.Entities.Abstractions;
using FastVocab.Domain.Entities.JunctionEntities;

namespace FastVocab.Domain.Entities.CoreEntities;

public class Topic : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string VnText { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsHiding { get; set; }
    public virtual ICollection<WordTopic>? Words { get; set; }
}
