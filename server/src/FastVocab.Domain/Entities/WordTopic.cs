using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities;

public class WordTopic : EntityBase<int>
{
    public int WordId { get; set; }
    public Word? Word { get; set; }
    public int TopicId { get; set; }
    public Topic? Topic { get; set; }
}
