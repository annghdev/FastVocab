using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities;

public class WordList : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int CollectionId { get; set; }
    public Collection? Collection { get; set; }

    public ICollection<WordListDetail>? Words { get; set; }
}
