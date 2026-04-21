namespace IdentityProvider.Application.DTOs;

public record PartialLoginValidateRequest(string Username, Dictionary<int, char> Positions);

public record PartialLoginValidateResponse(bool IsValid, string? SubjectId, string? Error);