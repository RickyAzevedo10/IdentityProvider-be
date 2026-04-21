namespace IdentityProvider.Domain.Entities;

public class User
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public required string SubjectId { get; init; }
    public required string[] Scopes { get; init; }
    public DateTime CreatedAt { get; init; }
    public ICollection<PasswordPartial> PasswordPartials { get; init; } = new List<PasswordPartial>();
}