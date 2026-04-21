using IdentityProvider.Domain.Entities;

namespace IdentityProvider.Infrastructure.Persistence.Interfaces;

public interface IUsernameRecordRepository : IRepository<UsernameRecord>
{
    Task<UsernameRecord?> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string username);
}