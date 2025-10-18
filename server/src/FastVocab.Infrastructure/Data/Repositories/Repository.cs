using FastVocab.Domain.Entities.Abstractions;
using FastVocab.Domain.Repositories;
using FastVocab.Infrastructure.Data.EFCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FastVocab.Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FindAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T?> GetWithInfoAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        // Apply includes (eager loading)
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public virtual T Add(T entity)
    {
        return _dbSet.Add(entity).Entity;
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Remove(T entity)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.IsDeleted = true;
            Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }
    }
}

