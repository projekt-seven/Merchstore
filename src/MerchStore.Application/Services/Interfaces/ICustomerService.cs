using System.Collections.Generic;
using System.Threading.Tasks;
using MerchStore.Application.DTOs;

namespace MerchStore.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerListItemDto>> GetCustomerListAsync();
    }
}
