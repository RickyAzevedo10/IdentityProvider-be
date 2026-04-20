using IdentityProvider.Domain.Entities;

namespace IdentityProvider.Domain.Interfaces;

/// <summary>
/// Abstraction for user storage and retrieval operations.
/// </summary>
public interface IUserStore
{
    /// <summary>
    /// Validates the credentials of a user.
    /// </summary>
    /// <param name="username">The username to validate.</param>
    /// <param name="password">The password to validate.</param>
    /// <returns>True if credentials are valid, false otherwise.</returns>
    Task<bool> ValidateCredentialsAsync(string username, string password);

    /// <summary>
    /// Finds a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The user if found, null otherwise.</returns>
    Task<User?> FindByUsernameAsync(string username);

    /// <summary>
    /// Checks if a user exists with the given username.
    /// </summary>
    /// <param name="username">The username to check.</param>
    /// <returns>True if the user exists, false otherwise.</returns>
    Task<bool> UserExistsAsync(string username);

    /// <summary>
    /// Registers a new user with the given credentials and scopes.
    /// </summary>
    /// <param name="username">The username for the new user.</param>
    /// <param name="password">The password for the new user.</param>
    /// <param name="scopes">The OAuth2 scopes to assign to the user.</param>
    /// <returns>True if registration succeeded, false if user already exists.</returns>
    Task<bool> RegisterUserAsync(string username, string password, string[] scopes);
}
