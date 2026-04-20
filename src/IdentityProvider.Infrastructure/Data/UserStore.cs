using System.Security.Cryptography;
using IdentityProvider.Domain.Entities;
using IdentityProvider.Domain.Interfaces;

namespace IdentityProvider.Infrastructure.Data;

/// <summary>
/// In-memory implementation of user storage.
/// </summary>
public class UserStore : IUserStore
{
    private readonly Dictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);

    public UserStore()
    {
        _users["Alice"] = new User
        {
            Username = "Alice",
            PasswordHash = HashPassword("password"),
            SubjectId = "1",
            Scopes = ["openid", "offline_access", "api1", "api2"]
        };
        _users["Bob"] = new User
        {
            Username = "Bob",
            PasswordHash = HashPassword("password"),
            SubjectId = "2",
            Scopes = ["openid", "api1"]
        };
    }

    public Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        if (!_users.TryGetValue(username, out var user))
            return Task.FromResult(false);

        return Task.FromResult(VerifyPassword(password, user.PasswordHash));
    }

    public Task<User?> FindByUsernameAsync(string username)
    {
        _users.TryGetValue(username, out var user);
        return Task.FromResult(user);
    }

    public Task<bool> UserExistsAsync(string username)
    {
        return Task.FromResult(_users.ContainsKey(username));
    }

    public Task<bool> RegisterUserAsync(string username, string password, string[] scopes)
    {
        if (_users.ContainsKey(username))
            return Task.FromResult(false);

        var subjectId = (_users.Count + 1).ToString();
        _users[username] = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            SubjectId = subjectId,
            Scopes = scopes
        };
        return Task.FromResult(true);
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);

        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }
}
