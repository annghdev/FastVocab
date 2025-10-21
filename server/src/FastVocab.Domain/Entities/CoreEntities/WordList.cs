using FastVocab.Domain.Entities.Abstractions;
using FastVocab.Domain.Entities.JunctionEntities;

namespace FastVocab.Domain.Entities.CoreEntities;

public class WordList : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int CollectionId { get; set; }
    public virtual Collection? Collection { get; set; }

    public virtual ICollection<WordListDetail> Words { get; set; } = [];
}
