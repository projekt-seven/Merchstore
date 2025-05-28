using System.Collections.Generic;
using System.Threading.Tasks;
using MerchStore.Application.DTOs;

namespace MerchStore.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderListItemDto>> GetOrderListAsync();
    }
}
