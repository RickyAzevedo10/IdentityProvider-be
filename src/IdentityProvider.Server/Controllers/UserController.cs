using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityProvider.Application.DTOs;
using IdentityProvider.Application.Interfaces;

namespace IdentityProvider.Server.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserRegistrationService _registrationService;

    public UserController(IUserRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost("validate-username")]
    public async Task<IActionResult> ValidateUsername([FromBody] ValidateUsernameRequest request)
    {
        var result = await _registrationService.ValidateUsernameAsync(request);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var result = await _registrationService.CreateUserAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}