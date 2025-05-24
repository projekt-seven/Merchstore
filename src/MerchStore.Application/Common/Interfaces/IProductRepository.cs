
using MerchStore.Domain.Entities;

namespace MerchStore.Application.Common.Interfaces
{
	public interface IProductRepository : IRepository<Product, Guid>
	{
		Task<IEnumerable<Product>> GetByCategoryAsync(string category);
		Task AddAsync(Product product);
		Task<Product?> GetByIdAsync(Guid id);
		Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken);
		Task<IEnumerable<Product>> GetAllAsync();
	}
}