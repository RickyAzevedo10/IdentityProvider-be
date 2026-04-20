namespace IdentityProvider.Application.DTOs;

/// <summary>
/// Result of a login operation.
/// </summary>
/// <param name="Success">Whether the login was successful.</param>
/// <param name="SubjectId">The subject identifier of the user.</param>
/// <param name="Username">The username of the user.</param>
/// <param name="Error">The error message if login failed.</param>
public record LoginResult(bool Success, string? SubjectId, string? Username, string? Error);
