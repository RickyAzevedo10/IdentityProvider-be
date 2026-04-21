using IdentityProvider.Domain.Entities;

namespace IdentityProvider.Domain.Interfaces;

public interface IUsernameStore
{
    Task<UsernameRecord?> GetByUsernameAsync(string username);
    Task<bool> ExistsAsync(string username);
    Task AddAsync(UsernameRecord record);
}