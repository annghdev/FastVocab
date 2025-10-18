using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Infrastructure.Data.EFCore;
using Microsoft.EntityFrameworkCore;

namespace FastVocab.Infrastructure.Data.Repositories;

public class WordRepository : Repository<Word>, IWordRepository
{
    public WordRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Word>> GetByTopic(int topicId)
    {
        return await _context.WordTopics
            .Where(wt => wt.TopicId == topicId)
            .Include(wt=>wt.Word)
            .Select(wt => wt.Word!)
            .ToListAsync();
    }
}

