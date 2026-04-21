using IdentityProvider.Domain.Entities;
using IdentityProvider.Infrastructure.Persistence.Context;
using IdentityProvider.Infrastructure.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .Include(u => u.PasswordPartials)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetBySubjectIdAsync(string subjectId)
    {
        return await _dbSet
            .Include(u => u.PasswordPartials)
            .FirstOrDefaultAsync(u => u.SubjectId == subjectId);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.PasswordPartials)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}