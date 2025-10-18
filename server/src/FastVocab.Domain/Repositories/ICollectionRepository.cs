using FastVocab.Domain.Entities.CoreEntities;

namespace FastVocab.Domain.Repositories;

public interface ICollectionRepository : IRepository<Collection>
{
    /// <summary>
    /// Get collection with its word lists included
    /// </summary>
    Task<Collection?> GetWithWordListsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all visible collections (not hidden, not deleted)
    /// </summary>
    Task<IEnumerable<Collection>> GetVisibleCollectionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get collections by difficulty level
    /// </summary>
    Task<IEnumerable<Collection>> GetByDifficultyLevelAsync(string level, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get collection with full details (word lists and their words)
    /// </summary>
    Task<Collection?> GetWithFullDetailsAsync(int id, CancellationToken cancellationToken = default);
}
