using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities;

public class Collection : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<WordList>? WordLists { get; set; }
}
