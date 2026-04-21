using IdentityProvider.Domain.Entities;
using IdentityProvider.Domain.Interfaces;
using IdentityProvider.Infrastructure.Persistence.Interfaces;

namespace IdentityProvider.Infrastructure.Services;

public class UserStore : IUserStore
{
    private readonly IUserRepository _userRepository;
    private readonly IRepository<PasswordPartial> _passwordPartialRepository;

    public UserStore(IUnitOfWork unitOfWork)
    {
        _userRepository = unitOfWork.Users;
        _passwordPartialRepository = unitOfWork.PasswordPartials;
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<string?> CreateUserAsync(string username, string password, string email, string? phoneNumber, string[] scopes)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser is not null)
            return null;

        var existingEmail = await _userRepository.GetByEmailAsync(email);
        if (existingEmail is not null)
            return null;

        var subjectId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid();
        var passwordPartials = GeneratePartialPasswordHashes(password, userId);

        var user = new User
        {
            Id = userId,
            Username = username,
            Email = email,
            PhoneNumber = phoneNumber,
            SubjectId = subjectId,
            Scopes = scopes,
            CreatedAt = DateTime.UtcNow,
            PasswordPartials = passwordPartials
        };

        await _userRepository.AddAsync(user);
        await _passwordPartialRepository.AddRangeAsync(passwordPartials);
        await _userRepository.SaveChangesAsync();

        return subjectId;
    }

    public async Task<PasswordPartial[]?> GetPartialPasswordChallengesAsync(string username, int count)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user is null)
            return null;

        var random = new Random();
        var shuffled = user.PasswordPartials.OrderBy(_ => random.Next()).Take(count).ToArray();

        return shuffled;
    }

    public async Task<bool> ValidatePartialPasswordAsync(string username, Dictionary<int, char> positions)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user is null)
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

    private static List<PasswordPartial> GeneratePartialPasswordHashes(string password, Guid userId)
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
                UserId = userId,
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