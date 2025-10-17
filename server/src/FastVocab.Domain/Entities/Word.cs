using FastVocab.Domain.Constants;
using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities;

public class Word : AuditableEntityBase<int>
{
    public string Text { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string Type { get; set; } = WordTypes.Noun;
    public string Level { get; set; } = WordLevels.A1;
    public string? Example1 { get; set; }
    public string? Example2 { get; set; }
    public string? Example3 { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }

    public ICollection<WordTopic>? Topics { get; set; }
}
