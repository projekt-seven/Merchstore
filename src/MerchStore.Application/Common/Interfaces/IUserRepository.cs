using MerchStore.Domain.Entities;

namespace MerchStore.Application.Common.Interfaces
{

    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<User?> GetByUsernameAsync(string username);
    }
}