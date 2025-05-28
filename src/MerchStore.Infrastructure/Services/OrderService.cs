using Microsoft.EntityFrameworkCore;
using MerchStore.Application.DTOs;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Infrastructure.Persistence;

namespace MerchStore.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderListItemDto>> GetOrderListAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Select(o => new OrderListItemDto
                {
                    Id = o.Id,
                    CustomerFullName = o.Customer.FirstName + " " + o.Customer.LastName,
                    TotalAmount = o.TotalPrice.Amount,
                    OrderDate = o.OrderDate,
                    Status = o.Status.ToString()
                })
                .ToListAsync();
        }
    }
}
