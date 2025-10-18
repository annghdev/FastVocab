namespace FastVocab.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITopicRepository Topics { get; }
    IWordRepository Words { get; }
    ICollectionRepository Collections { get; }
    IPracticeSessionRepository PracticeSessions { get; }
    ITakedWordRepository TakedWords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

