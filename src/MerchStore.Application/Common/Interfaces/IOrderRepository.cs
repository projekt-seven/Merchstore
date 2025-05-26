using MerchStore.Domain.Entities;

namespace MerchStore.Application.Common.Interfaces
{


    public interface IOrderRepository : IRepository<Order, Guid>
    {
        Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<List<Order>> GetOrdersByCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
    }
}