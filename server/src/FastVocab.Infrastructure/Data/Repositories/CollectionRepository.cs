using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Infrastructure.Data.EFCore;
using Microsoft.EntityFrameworkCore;

namespace FastVocab.Infrastructure.Data.Repositories;

public class CollectionRepository : Repository<Collection>, ICollectionRepository
{
    public CollectionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Collection?> GetWithWordListsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.WordLists)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Collection>> GetVisibleCollectionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => !c.IsHiding)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Collection>> GetByDifficultyLevelAsync(string level, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.DifficultyLevel == level)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Collection?> GetWithFullDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.WordLists)
                .ThenInclude(wl => wl.Words)
                    .ThenInclude(wld => wld.Word)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<WordList?> GetWordListAsync(int id)
    {
        return await _context.WordLists
            .Include(l => l.Words)
                .ThenInclude(dt=>dt.Word)
            .FirstOrDefaultAsync(l => l.Id == id);
    }
}

