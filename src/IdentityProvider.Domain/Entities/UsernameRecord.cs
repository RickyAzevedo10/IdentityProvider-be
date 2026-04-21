namespace IdentityProvider.Domain.Entities;

public class UsernameRecord
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public DateTime CreatedAt { get; init; }
}