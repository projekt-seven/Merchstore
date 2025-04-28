using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}