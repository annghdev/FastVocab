using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Infrastructure.Data.EFCore;

namespace FastVocab.Infrastructure.Data.Repositories;

public class TakedWordRepository : Repository<TakedWord>, ITakedWordRepository
{
    public TakedWordRepository(AppDbContext context) : base(context)
    {
    }
}

