using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MerchStore.Infrastructure.Persistence.Repositories;

public class OrderRepository : Repository<Order, Guid>, IOrderRepository
{
    public new readonly AppDbContext _context;

    public OrderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<List<Order>> GetOrdersByCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer, nameof(customer));

        return await _context.Orders
        .Include(o => o.Items)
        .Where(o => o.Customer.FirstName == customer.FirstName &&
                    o.Customer.LastName == customer.LastName &&
                    o.Customer.Email == customer.Email &&
                    o.Customer.PhoneNumber == customer.PhoneNumber &&
                    o.Customer.Address == customer.Address &&
                    o.Customer.City == customer.City &&
                    o.Customer.PostalCode == customer.PostalCode)
        .ToListAsync(cancellationToken);
    }

    
}