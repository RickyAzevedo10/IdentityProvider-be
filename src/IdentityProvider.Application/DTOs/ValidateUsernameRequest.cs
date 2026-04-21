namespace IdentityProvider.Application.DTOs;

public record ValidateUsernameRequest(string Username);

public record ValidateUsernameResponse(bool IsAvailable);