using IdentityProvider.Application.DTOs;

namespace IdentityProvider.Application.Interfaces;

public interface IAuthService
{
    Task<PartialLoginChallengeResponse> GetLoginChallengeAsync(PartialLoginChallengeRequest request);
    Task<PartialLoginValidateResponse> ValidatePartialLoginAsync(PartialLoginValidateRequest request);
}
