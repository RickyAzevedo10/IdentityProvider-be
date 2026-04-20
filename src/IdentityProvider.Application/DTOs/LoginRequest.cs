namespace IdentityProvider.Application.DTOs;

/// <summary>
/// Login request data transfer object.
/// </summary>
/// <param name="Username">The username for authentication.</param>
/// <param name="Password">The password for authentication.</param>
public record LoginRequest(string Username, string Password);
