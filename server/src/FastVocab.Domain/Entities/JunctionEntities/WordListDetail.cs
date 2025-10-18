using FastVocab.Domain.Entities.Abstractions;
using FastVocab.Domain.Entities.CoreEntities;

namespace FastVocab.Domain.Entities.JunctionEntities;

public class WordListDetail : EntityBase<int>
{
    public int WordListId { get; set; }
    public virtual WordList? WordList { get; set; }
    public int WordId { get; set; }
    public virtual Word? Word { get; set; }
}
