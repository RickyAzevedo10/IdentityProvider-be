namespace IdentityProvider.Domain.Entities;

/// <summary>
/// Represents a user in the authentication system.
/// </summary>
public class User
{
    /// <summary>
    /// The username of the user.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// The hashed password of the user.
    /// </summary>
    public required string PasswordHash { get; init; }

    /// <summary>
    /// The unique subject identifier for the user.
    /// </summary>
    public required string SubjectId { get; init; }

    /// <summary>
    /// The OAuth2 scopes assigned to the user.
    /// </summary>
    public required string[] Scopes { get; init; }
}
