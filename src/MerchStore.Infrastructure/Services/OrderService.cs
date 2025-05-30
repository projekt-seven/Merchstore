using Microsoft.EntityFrameworkCore;
using MerchStore.Application.DTOs;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
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

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.Customer.Id,
                TotalAmount = order.TotalPrice.Amount,
                Currency = order.TotalPrice.Currency,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString()
            };
        }

        public async Task CreateAsync(OrderDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
                throw new InvalidOperationException("Customer not found");

            var order = new Order(customer)
            {
                // Assuming you have public setters or internal setters EF can use
                OrderDate = dto.OrderDate,
                TotalPrice = new Money(dto.TotalAmount, dto.Currency),
                Status = Enum.Parse<OrderStatus>(dto.Status)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.Id);
            if (order == null)
                throw new InvalidOperationException("Order not found");

            // Här uppdaterar vi direkt via EF's change tracking
            order.TotalPrice = new Money(dto.TotalAmount, dto.Currency);
            order.OrderDate = dto.OrderDate;
            order.Status = Enum.Parse<OrderStatus>(dto.Status);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                throw new InvalidOperationException("Order not found");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
