using FastVocab.Domain.Entities.Abstractions;
using System.Linq.Expressions;

namespace FastVocab.Domain.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken = default);
    Task<T?> FindAsync(object id);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetWithInfoAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    T Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
