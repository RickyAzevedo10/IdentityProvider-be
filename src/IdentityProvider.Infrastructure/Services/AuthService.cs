using IdentityProvider.Application.DTOs;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Domain.Interfaces;

namespace IdentityProvider.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserStore _userStore;

    public AuthService(IUserStore userStore)
    {
        _userStore = userStore;
    }

    public async Task<PartialLoginChallengeResponse> GetLoginChallengeAsync(PartialLoginChallengeRequest request)
    {
        var challenges = await _userStore.GetPartialPasswordChallengesAsync(request.Username, 4);
        if (challenges is null)
            return new PartialLoginChallengeResponse(Array.Empty<int[]>());

        return new PartialLoginChallengeResponse(challenges.Select(c => c.PositionIndexes).ToArray()!);
    }

    public async Task<PartialLoginValidateResponse> ValidatePartialLoginAsync(PartialLoginValidateRequest request)
    {
        var isValid = await _userStore.ValidatePartialPasswordAsync(request.Username, request.Positions);
        if (!isValid)
            return new(false, null, "Invalid credentials.");

        var user = await _userStore.FindByUsernameAsync(request.Username);
        if (user is null)
            return new(false, null, "User not found.");

        return new(true, user.SubjectId, null);
    }
}