namespace IdentityProvider.Application.DTOs;

/// <summary>
/// Registration request data transfer object.
/// </summary>
/// <param name="Username">The username for the new account.</param>
/// <param name="Password">The password for the new account.</param>
public record RegisterRequest(string Username, string Password);
