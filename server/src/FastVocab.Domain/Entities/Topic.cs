using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities;

public class Topic : AuditableEntityBase<int>
{
    public int Name { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<WordTopic>? Words { get; set; }
}
