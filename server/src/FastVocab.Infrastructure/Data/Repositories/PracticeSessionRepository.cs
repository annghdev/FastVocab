using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Infrastructure.Data.EFCore;

namespace FastVocab.Infrastructure.Data.Repositories;

public class PracticeSessionRepository : Repository<PracticeSesssion>, IPracticeSessionRepository
{
    public PracticeSessionRepository(AppDbContext context) : base(context)
    {
    }
}

