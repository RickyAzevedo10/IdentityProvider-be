namespace IdentityProvider.Application.DTOs;

public record CreateUserRequest(string Username, string Password, string Email, string? PhoneNumber);

public record CreateUserResponse(bool Success, string? SubjectId, string? Error);