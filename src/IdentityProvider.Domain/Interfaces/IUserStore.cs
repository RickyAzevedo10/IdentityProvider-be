using IdentityProvider.Domain.Entities;

namespace IdentityProvider.Domain.Interfaces;

public interface IUserStore
{
    Task<User?> FindByUsernameAsync(string username);
    Task<string?> CreateUserAsync(string username, string password, string email, string? phoneNumber, string[] scopes);
    Task<PasswordPartial[]?> GetPartialPasswordChallengesAsync(string username, int count);
    Task<bool> ValidatePartialPasswordAsync(string username, Dictionary<int, char> positions);
}