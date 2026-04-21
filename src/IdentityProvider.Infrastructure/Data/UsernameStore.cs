using IdentityProvider.Domain.Entities;
using IdentityProvider.Domain.Interfaces;

namespace IdentityProvider.Infrastructure.Data;

public class UsernameStore : IUsernameStore
{
    private readonly Dictionary<string, UsernameRecord> _records = new(StringComparer.OrdinalIgnoreCase);

    public Task<UsernameRecord?> GetByUsernameAsync(string username)
    {
        _records.TryGetValue(username, out var record);
        return Task.FromResult<UsernameRecord?>(record);
    }

    public Task<bool> ExistsAsync(string username)
    {
        return Task.FromResult(_records.ContainsKey(username));
    }

    public Task AddAsync(UsernameRecord record)
    {
        _records[record.Username] = record;
        return Task.CompletedTask;
    }
}