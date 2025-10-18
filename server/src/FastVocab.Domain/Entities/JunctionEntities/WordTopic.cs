using FastVocab.Domain.Entities.Abstractions;
using FastVocab.Domain.Entities.CoreEntities;

namespace FastVocab.Domain.Entities.JunctionEntities;

public class WordTopic : EntityBase<int>
{
    public int WordId { get; set; }
    public virtual Word? Word { get; set; }
    public int TopicId { get; set; }
    public virtual Topic? Topic { get; set; }
}
