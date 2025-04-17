using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Interfaces;

public interface IProductRepository : IRepository<Product, Guid>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
}