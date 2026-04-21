namespace IdentityProvider.Domain.Entities;

public class PasswordPartial
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public required int[] PositionIndexes { get; init; }
    public required string Hash { get; init; }
}