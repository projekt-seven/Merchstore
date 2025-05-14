using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MerchStore.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User, Guid>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}