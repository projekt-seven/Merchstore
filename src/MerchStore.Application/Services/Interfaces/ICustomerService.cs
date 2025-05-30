using System.Collections.Generic;
using System.Threading.Tasks;
using MerchStore.Application.DTOs;

namespace MerchStore.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerListItemDto>> GetCustomerListAsync();
        Task<CustomerDto?> GetByIdAsync(Guid id);
        Task CreateAsync(CustomerDto dto);
        Task UpdateAsync(CustomerDto dto);
        Task DeleteAsync(Guid id);
    }
}
