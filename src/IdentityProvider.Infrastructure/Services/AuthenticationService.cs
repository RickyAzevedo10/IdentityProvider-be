using IdentityProvider.Application.DTOs;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Domain.Interfaces;

namespace IdentityProvider.Infrastructure.Services;

/// <summary>
/// Implementation of authentication business logic.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserStore _userStore;

    public AuthService(IUserStore userStore)
    {
        _userStore = userStore;
    }

    public async Task<LoginResult> LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return new(false, null, null, "Username and password are required.");

        var isValid = await _userStore.ValidateCredentialsAsync(username, password);
        if (!isValid)
            return new(false, null, null, "Invalid credentials.");

        var user = await _userStore.FindByUsernameAsync(username);
        if (user is null)
            return new(false, null, null, "Invalid credentials.");

        return new(true, user.SubjectId, user.Username, null);
    }

    public async Task<RegistrationResult> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return new(false, "Username and password are required.");

        if (password.Length < 4)
            return new(false, "Password must be at least 4 characters.");

        var exists = await _userStore.UserExistsAsync(username);
        if (exists)
            return new(false, "Username already exists.");

        var scopes = new[] { "openid", "offline_access", "api1" };
        await _userStore.RegisterUserAsync(username, password, scopes);

        return new(true, null);
    }
}
