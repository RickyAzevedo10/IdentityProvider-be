using IdentityProvider.Application.DTOs;

namespace IdentityProvider.Application.Interfaces;

public interface IUserRegistrationService
{
    Task<ValidateUsernameResponse> ValidateUsernameAsync(ValidateUsernameRequest request);
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
}