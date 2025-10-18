using FastVocab.Domain.Entities.Abstractions;
using FastVocab.Domain.Repositories;
using FastVocab.Infrastructure.Data.EFCore;
using Microsoft.EntityFrameworkCore;

namespace FastVocab.Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public ITopicRepository Topics { get; }
    public IWordRepository Words { get; }
    public ICollectionRepository Collections { get; }
    public IPracticeSessionRepository PracticeSessions { get; }
    public ITakedWordRepository TakedWords { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;

        Users = new UserRepository(context);
        Topics = new TopicRepository(context);
        Words = new WordRepository(context);
        Collections = new CollectionRepository(context);
        PracticeSessions = new PracticeSessionRepository(context);
        TakedWords = new TakedWordRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();

        return await _context.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        var entries = _context.ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditable &&
                   (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (IAuditable)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

