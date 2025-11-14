using Domain.Entities;
using System.Linq.Expressions;

namespace Infrastructure.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    // ADD Methods
    Task<TEntity> AddAsync(TEntity entity);
    Task AddRangeAsync(List<TEntity> entities);

    // QUERY Methods
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetByIdAsync(string id, params Expression<Func<TEntity, object>>[] includes);
    Task<IQueryable<TEntity>> GetQueryableAsync();

    // UPDATE Methods
    Task<bool> Update(TEntity entity);
    Task<bool> UpdateRange(List<TEntity> entities);
}