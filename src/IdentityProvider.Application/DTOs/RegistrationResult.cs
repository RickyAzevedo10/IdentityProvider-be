namespace IdentityProvider.Application.DTOs;

/// <summary>
/// Result of a registration operation.
/// </summary>
/// <param name="Success">Whether the registration was successful.</param>
/// <param name="Error">The error message if registration failed.</param>
public record RegistrationResult(bool Success, string? Error);
