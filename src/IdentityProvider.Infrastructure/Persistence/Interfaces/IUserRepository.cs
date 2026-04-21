using IdentityProvider.Domain.Entities;

namespace IdentityProvider.Infrastructure.Persistence.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetBySubjectIdAsync(string subjectId);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
}