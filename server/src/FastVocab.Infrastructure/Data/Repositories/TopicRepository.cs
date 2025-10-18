using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Infrastructure.Data.EFCore;

namespace FastVocab.Infrastructure.Data.Repositories;

public class TopicRepository : Repository<Topic>, ITopicRepository
{
    public TopicRepository(AppDbContext context) : base(context)
    {
    }
}

