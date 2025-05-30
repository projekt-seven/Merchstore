using System.Collections.Generic;
using System.Threading.Tasks;
using MerchStore.Application.DTOs;

namespace MerchStore.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderListItemDto>> GetOrderListAsync();
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task CreateAsync(OrderDto dto);
        Task UpdateAsync(OrderDto dto);
        Task DeleteAsync(Guid id);
    }
}
