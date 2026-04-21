using BCrypt.Net;
using IdentityProvider.Domain.Entities;
using IdentityProvider.Domain.Interfaces;

namespace IdentityProvider.Infrastructure.Data;

public class UserStore : IUserStore
{
    private readonly Dictionary<string, User> _users = new(StringComparer.OrdinalIgnoreCase);

    public Task<User?> FindByUsernameAsync(string username)
    {
        _users.TryGetValue(username, out var user);
        return Task.FromResult(user);
    }

    public async Task<string?> CreateUserAsync(string username, string password, string email, string? phoneNumber, string[] scopes)
    {
        if (_users.ContainsKey(username))
            return null;

        var subjectId = Guid.NewGuid().ToString();
        var passwordPartials = GeneratePartialPasswordHashes(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PhoneNumber = phoneNumber,
            SubjectId = subjectId,
            Scopes = scopes,
            CreatedAt = DateTime.UtcNow,
            PasswordPartials = passwordPartials
        };

        _users[username] = user;
        return subjectId;
    }

    public Task<PasswordPartial[]?> GetPartialPasswordChallengesAsync(string username, int count)
    {
        if (!_users.TryGetValue(username, out var user))
            return Task.FromResult<PasswordPartial[]?>(null);

        var random = new Random();
        var shuffled = user.PasswordPartials.OrderBy(_ => random.Next()).Take(count).ToArray();

        return Task.FromResult<PasswordPartial[]?>(shuffled);
    }

    public async Task<bool> ValidatePartialPasswordAsync(string username, Dictionary<int, char> positions)
    {
        if (!_users.TryGetValue(username, out var user))
            return false;

        foreach (var (index, expectedChar) in positions)
        {
            var partial = user.PasswordPartials.FirstOrDefault(p => 
                p.PositionIndexes.Length == 1 && p.PositionIndexes[0] == index);

            if (partial is null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(expectedChar.ToString(), partial.Hash))
                return false;
        }

        return true;
    }

    private static List<PasswordPartial> GeneratePartialPasswordHashes(string password)
    {
        var combinations = GetCombinations(8, 4);
        var partials = new List<PasswordPartial>();

        foreach (var combo in combinations)
        {
            var chars = combo.Select(i => password[i]).ToArray();
            var combinationString = new string(chars);
            var hash = BCrypt.Net.BCrypt.HashPassword(combinationString, BCrypt.Net.BCrypt.GenerateSalt(12));

            partials.Add(new PasswordPartial
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Empty,
                PositionIndexes = combo.ToArray(),
                Hash = hash
            });
        }

        return partials;
    }

    private static IEnumerable<int[]> GetCombinations(int n, int k)
    {
        var result = new List<int[]>();
        var indices = Enumerable.Range(0, k).ToArray();

        void Recurse(int start)
        {
            if (k == indices.Length)
            {
                result.Add(indices.ToArray());
                return;
            }

            for (var i = start; i < n; i++)
            {
                indices[k] = i;
                Recurse(i + 1);
            }
        }

        Recurse(0);
        return result;
    }
}