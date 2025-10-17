using FastVocab.Domain.Entities.Abstractions;

namespace FastVocab.Domain.Entities;

public class WordListDetail : EntityBase<int>
{
    public int WordListId { get; set; }
    public WordList? WordList { get; set; }
    public int WordId { get; set; }
    public Word? Word { get; set; }
}
