using IdentityProvider.Domain.Entities;
using IdentityProvider.Infrastructure.Persistence.Context;
using IdentityProvider.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Infrastructure.Persistence.Repositories;

public class UsernameRecordRepository : Repository<UsernameRecord>, IUsernameRecordRepository
{
    public UsernameRecordRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UsernameRecord?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }
}