using FastVocab.Domain.Entities.CoreEntities;

namespace FastVocab.Domain.Repositories;

public interface IWordRepository : IRepository<Word>
{
    Task<IEnumerable<Word>> GetByTopic(int  topicId);
}
