using IdentityProvider.Infrastructure.Persistence.Context;
using IdentityProvider.Infrastructure.Persistence.Interfaces;
using IdentityProvider.Infrastructure.Persistence.Repositories;

namespace IdentityProvider.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IUserRepository? _users;
    private IUsernameRecordRepository? _usernameRecords;
    private IRepository<Domain.Entities.PasswordPartial>? _passwordPartials;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IUsernameRecordRepository UsernameRecords => _usernameRecords ??= new UsernameRecordRepository(_context);
    public IRepository<Domain.Entities.PasswordPartial> PasswordPartials => _passwordPartials ??= new Repository<Domain.Entities.PasswordPartial>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}