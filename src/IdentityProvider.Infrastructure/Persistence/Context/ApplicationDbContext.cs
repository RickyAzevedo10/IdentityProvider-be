using IdentityProvider.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<PasswordPartial> PasswordPartials => Set<PasswordPartial>();
    public DbSet<UsernameRecord> UsernameRecords => Set<UsernameRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}