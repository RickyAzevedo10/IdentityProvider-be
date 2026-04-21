using IdentityProvider.Domain.Entities;
using IdentityProvider.Domain.Interfaces;
using IdentityProvider.Infrastructure.Persistence.Interfaces;

namespace IdentityProvider.Infrastructure.Services;

public class UsernameStore : IUsernameStore
{
    private readonly IUsernameRecordRepository _usernameRecordRepository;

    public UsernameStore(IUnitOfWork unitOfWork)
    {
        _usernameRecordRepository = unitOfWork.UsernameRecords;
    }

    public async Task<UsernameRecord?> GetByUsernameAsync(string username)
    {
        return await _usernameRecordRepository.GetByUsernameAsync(username);
    }

    public async Task<bool> ExistsAsync(string username)
    {
        return await _usernameRecordRepository.ExistsByUsernameAsync(username);
    }

    public async Task AddAsync(UsernameRecord record)
    {
        await _usernameRecordRepository.AddAsync(record);
        await _usernameRecordRepository.SaveChangesAsync();
    }
}