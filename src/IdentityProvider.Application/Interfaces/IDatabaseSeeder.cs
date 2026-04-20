namespace IdentityProvider.Application.Interfaces;

/// <summary>
/// Abstraction for seeding initial data into the database.
/// </summary>
public interface IDatabaseSeeder
{
    /// <summary>
    /// Seeds the database with initial data.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SeedAsync();
}
