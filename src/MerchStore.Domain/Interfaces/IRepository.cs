using MerchStore.Domain.Common;

namespace MerchStore.Domain.Interfaces;

/* public interface IRepository1<TEntity, TId>
where TEntity : Entity<TId>
where TId : notnull
{
	Task AddAsync(TEntity entity);
	Task<IEnumerable<TEntity>> GetAllAsync();
	Task<TEntity?> GetByIdAsync(TId id);
	Task RemoveAsync(TEntity entity);
	Task UpdateAsync(TEntity entity);
}
 */
// Generic repository interface for standard CRUD operations
public interface IRepository<TEntity, TId>
	where TEntity : Entity<TId>
	where TId : notnull
{
	Task<TEntity?> GetByIdAsync(TId id);
	Task<IEnumerable<TEntity>> GetAllAsync();
	Task AddAsync(TEntity entity);
	Task UpdateAsync(TEntity entity);
	Task RemoveAsync(TEntity entity);
}