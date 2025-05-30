using Microsoft.EntityFrameworkCore;
using MerchStore.Application.DTOs;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Entities;
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

        public async Task<CustomerDto?> GetByIdAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return null;

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode
            };
        }

        public async Task CreateAsync(CustomerDto dto)
        {
            var customer = new Customer(
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.PhoneNumber,
                dto.Address,
                dto.City,
                dto.PostalCode
            );

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomerDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.Id);

            if (customer == null)
                throw new InvalidOperationException("Customer not found");

            // Eftersom dina properties har private set använder vi en workaround:
            var updatedCustomer = new Customer(
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.PhoneNumber,
                dto.Address,
                dto.City,
                dto.PostalCode
            );

            _context.Entry(customer).CurrentValues.SetValues(updatedCustomer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                throw new InvalidOperationException("Customer not found");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
