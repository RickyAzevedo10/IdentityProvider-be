namespace IdentityProvider.Infrastructure.Persistence.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IUsernameRecordRepository UsernameRecords { get; }
    IRepository<Domain.Entities.PasswordPartial> PasswordPartials { get; }
    Task<int> SaveChangesAsync();
}