using IdentityProvider.Application.DTOs;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Domain.Entities;
using IdentityProvider.Domain.Interfaces;

namespace IdentityProvider.Application.Services;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly IUsernameStore _usernameStore;
    private readonly IUserStore _userStore;

    public UserRegistrationService(IUsernameStore usernameStore, IUserStore userStore)
    {
        _usernameStore = usernameStore;
        _userStore = userStore;
    }

    public async Task<ValidateUsernameResponse> ValidateUsernameAsync(ValidateUsernameRequest request)
    {
        var exists = await _usernameStore.ExistsAsync(request.Username);
        return new ValidateUsernameResponse(!exists);
    }

    public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return new(false, null, "Username is required.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length != 8)
            return new(false, null, "Password must be exactly 8 characters.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return new(false, null, "Email is required.");

        var usernameExists = await _usernameStore.ExistsAsync(request.Username);
        if (usernameExists)
            return new(false, null, "Username already exists.");

        var scopes = new[] { "openid", "offline_access", "api1" };
        var subjectId = await _userStore.CreateUserAsync(
            request.Username,
            request.Password,
            request.Email,
            request.PhoneNumber,
            scopes);

        if (subjectId is null)
            return new(false, null, "Failed to create user.");

        var usernameRecord = new UsernameRecord
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            CreatedAt = DateTime.UtcNow
        };
        await _usernameStore.AddAsync(usernameRecord);
        return new(true, subjectId, null);
    }
}