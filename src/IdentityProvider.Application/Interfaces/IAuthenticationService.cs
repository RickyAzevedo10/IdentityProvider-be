using IdentityProvider.Application.DTOs;

namespace IdentityProvider.Application.Interfaces;

/// <summary>
/// Abstraction for authentication business logic.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user with the given credentials.
    /// </summary>
    /// <param name="username">The username to authenticate.</param>
    /// <param name="password">The password to validate.</param>
    /// <returns>A LoginResult indicating success or failure.</returns>
    Task<LoginResult> LoginAsync(string username, string password);

    /// <summary>
    /// Registers a new user with the given credentials.
    /// </summary>
    /// <param name="username">The username for the new account.</param>
    /// <param name="password">The password for the new account.</param>
    /// <returns>A RegistrationResult indicating success or failure.</returns>
    Task<RegistrationResult> RegisterAsync(string username, string password);
}
