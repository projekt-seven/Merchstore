using Microsoft.EntityFrameworkCore;
using MerchStore.Application.DTOs;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Infrastructure.Persistence;

namespace MerchStore.Infrastructure.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerListItemDto>> GetCustomerListAsync()
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .Select(c => new CustomerListItemDto
                {
                    Id = c.Id,
                    FullName = c.FirstName + " " + c.LastName,
                    Email = c.Email,
                    OrderCount = c.Orders.Count
                })
                .ToListAsync();
        }
    }
}
