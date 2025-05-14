using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Interfaces;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByUsernameAsync(string username);
}